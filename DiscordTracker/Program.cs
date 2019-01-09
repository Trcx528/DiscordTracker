using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using DiscordTracker.Data;
using System.Linq;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Collections.Generic;
using System.Text;
using DiscordTracker.Commands;

namespace DiscordTracker
{
    class Program
    {
        public static readonly DiscordSocketClient _client = new DiscordSocketClient();
        public static ApplicationDataContext _db = new ApplicationDataContext();
        public static IEnumerable<DiscordVoiceChannel> _discordVoiceChannels;
        public static IEnumerable<DiscordUser> _discordUsers;

#if DEBUG
        public const string triggerCharacter = "|";
#else
        public const string triggerCharacter = "!";
#endif


        private readonly CancellationTokenSource MainThread = new CancellationTokenSource();

        static void Main(string[] args)
        {
            //automatically attempt to apply any pending migrations on startup
            _db.Database.Migrate();
            _discordUsers = _db.DiscordUser.ToArray();
            _discordVoiceChannels = _db.DiscordVoiceChannel.ToArray();
            new Program().MainAsync().GetAwaiter().GetResult();
        }

        public Program()
        {
            _client.Ready += ReadyAsync;
            _client.GuildMemberUpdated += GuildUserUpdatedAsync;
            _client.MessageReceived += MessageReceievedAsync;
            _client.UserVoiceStateUpdated += UserVoiceStateUpdatedAsync;
            AppDomain.CurrentDomain.ProcessExit += BotShuttingDown;
        }

        private async void BotShuttingDown(object sender, EventArgs e)
        {
            foreach(var chan in _db.DiscordVoiceChannel)
            {
                var c = _client.GetChannel(chan.Id);
                foreach (var u in c.Users)
                {
                    await VoiceEventLog.Log(u, c, "Left");
                }
            }
            await _db.SaveChangesAsync();
            _client.Dispose();
            MainThread.Cancel();
            Environment.Exit(0);
        }

        private async Task GuildUserUpdatedAsync(SocketUser prevUser, SocketUser newUser)
        {
            await Log.LogAsync(newUser, $"{newUser.Username} Status change {newUser.Status} {newUser.Game}");
            _db.Add(new DiscordUserEvent() { EventTime = DateTime.Now, Game = newUser.Game?.Name, Status = newUser.Status.ToString(), UserId = newUser.Id });
            await _db.SaveChangesAsync(); 

        }

        private async Task UserVoiceStateUpdatedAsync(SocketUser user, SocketVoiceState prevState, SocketVoiceState newState)
        {
            if (prevState.VoiceChannel == newState.VoiceChannel)
            {
                if (newState.IsDeafened != prevState.IsDeafened)
                {
                    await Log.LogAsync(user, newState.VoiceChannel, newState.IsDeafened ? "Admin Deafened" : "Admin Undeafened");
                    await VoiceEventLog.Log(user, newState.VoiceChannel, newState.IsDeafened ? "Admin Deafened" : "Admin Undeafened");
                }
                else if (newState.IsMuted != prevState.IsMuted)
                {
                    await Log.LogAsync(user, newState.VoiceChannel, newState.IsMuted ? "Admin Muted" : "Admin Unmuted");
                    await VoiceEventLog.Log(user, newState.VoiceChannel, newState.IsMuted ? "Admin Muted" : "Admin Unmuted");
                } 
                else if (newState.IsSelfDeafened != prevState.IsSelfDeafened) //It's important that deafened check comes before mute check
                {
                    await Log.LogAsync(user, newState.VoiceChannel, newState.IsSelfDeafened ? "Self Deafened" : "Self Undeafened");
                    await VoiceEventLog.Log(user, newState.VoiceChannel, newState.IsSelfDeafened ? "Self Deafened" : "Self Undeafened");
                }
                else if (newState.IsSelfMuted != prevState.IsSelfMuted)
                {
                    await Log.LogAsync(user, newState.VoiceChannel, newState.IsSelfMuted ? "Self Muted" : "Self Unmuted");
                    await VoiceEventLog.Log(user, newState.VoiceChannel, newState.IsSelfMuted ? "Self Muted" : "Self Unmuted");
                } 
            }
            else if (prevState.VoiceChannel == null)
            {
                await Log.LogAsync(user, newState.VoiceChannel, "Joined");
                await VoiceEventLog.Log(user, newState.VoiceChannel, "Joined");
            }
            else if (newState.VoiceChannel == null)
            {
                await Log.LogAsync(user, prevState.VoiceChannel, "Left");
                await VoiceEventLog.Log(user, prevState.VoiceChannel, "Left");
            }
            else if (newState.VoiceChannel != prevState.VoiceChannel)
            {
                await Log.LogAsync(user, prevState.VoiceChannel, "Left");
                await VoiceEventLog.Log(user, prevState.VoiceChannel, "Left");
                await Log.LogAsync(user, newState.VoiceChannel, "Joined");
                await VoiceEventLog.Log(user, newState.VoiceChannel, "Joined");
            }
            else
            {
                await Log.LogAsync(user, newState.VoiceChannel, "Unknown");
            }
        }
        

        public async Task MainAsync()
        {
            var token = await _db.Settings.FirstOrDefaultAsync(s => s.Id == "DISCORD_TOKEN");
            if (token == null || token.Value == "")
            {
                Console.WriteLine("Please enter the discord token:");
                if (token == null)
                {
                    token = new Setting() { Id = "DISCORD_TOKEN" };
                    _db.Add(token);
                }
                token.Value = Console.ReadLine();
                await _db.SaveChangesAsync();
            }
            await _client.LoginAsync(Discord.TokenType.Bot, token.Value);
            await _client.StartAsync();
            await Task.Delay(-1, MainThread.Token);
        }

        private async Task ReadyAsync()
        {
            await Log.LogAsync($"{_client.CurrentUser} connected!");

            foreach (var chan in _db.DiscordVoiceChannel)
            {
                var c = _client.GetChannel(chan.Id);
                foreach (var u in c.Users)
                {
                    //TODO make this inteligent and check if they already have an open session
                    await VoiceEventLog.Log(u, c, "Joined");
                }
            }
            await _db.SaveChangesAsync();
        }
        
        private async Task CmdStatsAsync(SocketMessage message)
        {
            var stats = await _db.CallStats.OrderByDescending(s => s.TimeInCall).ToListAsync();
            var response = new StringBuilder();
            var length = stats.Select(s => s.User).OrderByDescending(s => s.Length).First().Length;
            response.Append($"```{"".PadLeft(length)}   Time  |  Muted | Deafened\n");
            foreach (var s in stats)
            {
                response.Append($"{s.User.PadLeft(length)}: {String.Format("{0:0.##}", Math.Round(s.TimeInCall, 2)).PadLeft(6)} | {String.Format("{0:0.##}", Math.Round(s.TimeMuted, 2)).PadLeft(6)} | {String.Format("{0:0.##}", Math.Round(s.TimeDeafened, 2)).PadLeft(8)}\n");
            }
            response.Append("```");
            await message.Channel.SendMessageAsync(response.ToString());
        }

        private async Task CmdCsvAsync(SocketMessage message)
        {
            var ms = new MemoryStream();
            var sw = new StreamWriter(ms);
            await sw.WriteLineAsync("Channel, Username, Start, End, Duration, Event Type");
            foreach (var cl in await _db.CallStatsDetails.ToListAsync())
            {
                await sw.WriteLineAsync($"{cl.Channel}, {cl.User}, {cl.Start}, {cl.End}, {cl.End - cl.Start}, {cl.EventType}");
            }
            sw.Flush();
            ms.Seek(0, SeekOrigin.Begin);
            await message.Channel.SendFileAsync(ms, "CallLogs.csv");
        }

        private async Task MessageReceievedAsync(SocketMessage message)
        {
            if (message.Author.Id == _client.CurrentUser.Id)
                return;

            if (message.Content.StartsWith(triggerCharacter))
            {
                var cmd = message.Content.Substring(1, message.Content.Length - 1).Split(' ').First();

                switch (cmd)
                {
                    case "ping":
                        await message.Channel.SendMessageAsync("pong!");
                        break;

                    case "csv":
                        await CmdCsvAsync(message);
                        break;

                    case "stats":
                        await CmdStatsAsync(message);
                        break;

                    case "quote":
                        await Quotes.Execute(message);
                        break;

                    case "reload":
                        if (message.Author.IsAdmin())
                            Program._db = new ApplicationDataContext();
                        else
                            await message.Channel.SendMessageAsync("You do not have permission to do that");
                        break;

                    default:
                        await message.Channel.SendMessageAsync("Unrecognized Commaned");
                        break;
                }
                await Log.LogAsync(message.Author, $"Recieved: {message.Content}");
            }
        }
    }
}
