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
        public static IEnumerable<DiscordVoiceChannel> _discordVoiceChannels;
        public static IEnumerable<DiscordUser> _discordUsers;
        public static IEnumerable<DiscordChannel> _discordChannels;
        internal static IEnumerable<object> _discordReactions;

#if DEBUG
        public const string triggerCharacter = "|";
#else
        public const string triggerCharacter = "!";
#endif


        private readonly CancellationTokenSource MainThread = new CancellationTokenSource();

        static void Main(string[] args)
        {
            //automatically attempt to apply any pending migrations on startup
            var db = new ApplicationDataContext();
            db.Database.Migrate();
            _discordUsers = db.DiscordUser.ToArray();
            _discordVoiceChannels = db.DiscordVoiceChannel.ToArray();
            _discordChannels = db.DiscordChannels.ToArray();
            db.Dispose();
            Minecraft.Start();
            new Program().MainAsync().GetAwaiter().GetResult();
        }

        public Program()
        {
            _client.Ready += ReadyAsync;
            _client.GuildMemberUpdated += GuildUserUpdatedAsync;
            _client.MessageReceived += MessageReceievedAsync;
            _client.UserVoiceStateUpdated += UserVoiceStateUpdatedAsync;
            _client.ReactionAdded += ReactionAddedAsync;
            _client.ReactionRemoved += ReactionRemovedAsync;
            _client.MessageDeleted += MessageDeletedAsync;
            AppDomain.CurrentDomain.ProcessExit += BotShuttingDown;
        }

        private async Task MessageDeletedAsync(Cacheable<IMessage, ulong> arg1, ISocketMessageChannel arg2)
        {
            using (var db = new ApplicationDataContext())
            {
                var msg = await DiscordMessage.CreateOrGetAsync(await arg1.GetOrDownloadAsync());
                db.Entry(msg);
                msg.IsDeleted = true;
                await db.SaveChangesAsync();
            }
        }

        private async Task ReactionRemovedAsync(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2, SocketReaction arg3)
        {
            Console.WriteLine($"Removing Reaction {arg3.Emote.Name}");
            using (var db = new ApplicationDataContext())
            {
                await DiscordMessage.CreateOrGetAsync(await arg1.GetOrDownloadAsync());
                var reaction = db.UserMessageReactions.FirstOrDefault(r => r.MessageId == arg3.MessageId && r.ReactorId == arg3.UserId && r.Reaction == arg3.Emote.Name);
                if (reaction != null)
                {
                    db.Remove(reaction);
                    await db.SaveChangesAsync();
                }
            }

        }

        private async Task ReactionAddedAsync(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2, SocketReaction arg3)
        {
            Console.WriteLine($"Adding Reaction {arg3.Emote.Name}");
            using (var db = new ApplicationDataContext())
            {
                await DiscordMessage.CreateOrGetAsync(await arg1.GetOrDownloadAsync());
                var reaction = new UserMessageReaction() { Created = DateTime.UtcNow, MessageId = arg3.MessageId, ReactorId = arg3.UserId, Reaction = arg3.Emote.Name };
                db.Add(reaction);
                await db.SaveChangesAsync();
            }
        }

        private async void BotShuttingDown(object sender, EventArgs e)
        {
            Console.WriteLine("Shutting Down");
            var db = new ApplicationDataContext();
            foreach (var chan in db.DiscordVoiceChannel)
            {
                var c = _client.GetChannel(chan.Id);
                foreach (var u in c.Users)
                {
                    await VoiceEventLog.Log(u, c, "Left");
                }
            }
            await db.SaveChangesAsync();
            _client.Dispose();
            MainThread.Cancel();
            Environment.Exit(0);
        }

        private async Task GuildUserUpdatedAsync(SocketUser prevUser, SocketUser newUser)
        {
            var db = new ApplicationDataContext();
            await Log.LogAsync(newUser, $"{newUser.Username} Status change {newUser.Status} {newUser.Activity.Name}");
            db.Add(new DiscordUserEvent() { EventTime = DateTime.Now, Game = newUser.Activity.Name, Status = newUser.Status.ToString(), UserId = newUser.Id });
            await db.SaveChangesAsync();
            db.Dispose();

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
            var db = new ApplicationDataContext();
            var token = await db.Settings.FirstOrDefaultAsync(s => s.Id == "DISCORD_TOKEN");
            if (token == null || token.Value == "")
            {
                Console.WriteLine("Please enter the discord token:");
                if (token == null)
                {
                    token = new Setting() { Id = "DISCORD_TOKEN" };
                    db.Add(token);
                }
                token.Value = Console.ReadLine();
                await db.SaveChangesAsync();
            }
            db.Dispose();
            await _client.LoginAsync(Discord.TokenType.Bot, token.Value);
            await _client.StartAsync();
            await Task.Delay(-1, MainThread.Token);
        }

        private async Task ReadyAsync()
        {
            await Log.LogAsync($"{_client.CurrentUser} connected!");

            var db = new ApplicationDataContext();
            foreach (var chan in db.DiscordVoiceChannel)
            {
                var c = _client.GetChannel(chan.Id);
                foreach (var u in c.Users)
                {
                    //TODO make this inteligent and check if they already have an open session
                    await VoiceEventLog.Log(u, c, "Joined");
                }
            }
            db.Dispose();
        }
        
        private async Task CmdStatsAsync(SocketMessage message)
        {
            var db = new ApplicationDataContext();
            var stats = await db.CallStats.OrderByDescending(s => s.TimeInCall).ToListAsync();
            var response = new StringBuilder();
            var length = stats.Select(s => s.User).OrderByDescending(s => s.Length).First().Length;
            response.Append($"```{"".PadLeft(length)}   Time  |  Muted | Deafened\n");
            foreach (var s in stats)
            {
                response.Append($"{s.User.PadLeft(length)}: {String.Format("{0:0.##}", Math.Round(s.TimeInCall, 2)).PadLeft(6)} | {String.Format("{0:0.##}", Math.Round(s.TimeMuted, 2)).PadLeft(6)} | {String.Format("{0:0.##}", Math.Round(s.TimeDeafened, 2)).PadLeft(8)}\n");
            }
            response.Append("```");
            await message.Channel.SendMessageAsync(response.ToString());
            db.Dispose();
        }

        private async Task CmdCsvAsync(SocketMessage message)
        {
            var ms = new MemoryStream();
            var sw = new StreamWriter(ms);
            await sw.WriteLineAsync("Channel, Username, Start, End, Duration, Event Type");
            var db = new ApplicationDataContext();
            foreach (var cl in await db.CallStatsDetails.ToListAsync())
            {
                await sw.WriteLineAsync($"{cl.Channel}, {cl.User}, {cl.Start}, {cl.End}, {cl.End - cl.Start}, {cl.EventType}");
            }
            sw.Flush();
            ms.Seek(0, SeekOrigin.Begin);
            await message.Channel.SendFileAsync(ms, "CallLogs.csv");
            db.Dispose();
        }

        private async Task MessageReceievedAsync(SocketMessage message)
        {
            if (message.Author.Id == _client.CurrentUser.Id)
                return;


            await DiscordChannel.CreateOrGetAsync(message.Channel);
            await DiscordMessage.CreateAsync(message);

            try
            {
                if (message.Content.StartsWith(triggerCharacter))
                {
                    var cmd = message.Content.Substring(1, message.Content.Length - 1).Split(' ').First();
                    switch (cmd)
                    {
                        case "ping":
                            await message.Channel.SendMessageAsync("pong!");
                            break;

                        case "test":
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
                            {
                                var _db = new ApplicationDataContext();
                                _discordUsers = await _db.DiscordUser.ToListAsync();
                                _discordVoiceChannels = await _db.DiscordVoiceChannel.ToListAsync();
                            }
                            else
                            {
                                await message.Channel.SendMessageAsync("You do not have permission to do that");
                            }
                            break;

                        default:
                            await message.Channel.SendMessageAsync("Unrecognized Commaned");
                            break;
                    }
                    await Log.LogAsync(message.Author, $"Recieved: {message.Content}");
                }
            } catch (Exception ex)
            {
                await message.Channel.SendMessageAsync("An Error Occured");
                await Log.LogAsync(ex.Message + "\n" + ex.StackTrace);
            }
        }
    }
}
