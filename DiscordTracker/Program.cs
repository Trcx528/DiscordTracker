using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using DiscordTracker.Data;
using System.Linq;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace DiscordTracker
{
    class Program
    {
        public static readonly DiscordSocketClient _client = new DiscordSocketClient();
        public static readonly ApplicationDataContext _db = new ApplicationDataContext();


        private readonly CancellationTokenSource MainThread = new CancellationTokenSource();

        static void Main(string[] args)
        {
            //automatically attempt to apply any pending migrations on startup
            _db.Database.Migrate();

            new Program().MainAsync().GetAwaiter().GetResult();
        }

        public Program()
        {
            _client.Ready += ReadyAsync;
            _client.UserUpdated += UserUpdatedAsync;
            _client.MessageReceived += MessageReceievedAsync;
            _client.UserVoiceStateUpdated += UserVoiceStateUpdatedAsync;
        }

        private async Task UserUpdatedAsync(SocketUser prevUser, SocketUser newUser)
        {
            await Log.LogAsync(prevUser, $"User Updated {prevUser.Status} {newUser.Status} {prevUser.Game} {newUser.Game}");
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
        }

        private async Task CmdStopAsync()
        {
            //shutting down
            _client.Dispose();
            MainThread.Cancel();
            Environment.Exit(0);
        }

        private async Task CmdStatsAsync(SocketMessage message)
        {
        }

        private async Task CmdCsvAsync(SocketMessage message)
        {
            var ms = new MemoryStream();
            var sw = new StreamWriter(ms);
            await sw.WriteLineAsync("Id, Channel, Username, JoinTime, LeaveTime, TotalTime, InCallBeforeJoined, InCallAfterLeft");
            foreach (var cl in await _db.VoiceEventLog.ToListAsync())
            {
                //await sw.WriteLineAsync($"{cl.Id}, {cl.Channel}, {cl.Username}, {cl.JoinTime}, {cl.LeaveTime}, {cl.TotalTime}, {cl.InCallBeforeJoined}, {cl.InCallAfterLeft}");
            }
            sw.Flush();
            ms.Seek(0, SeekOrigin.Begin);
            await message.Channel.SendFileAsync(ms, "CallLogs.csv");
        }

        private async Task MessageReceievedAsync(SocketMessage message)
        {
            if (message.Author.Id == _client.CurrentUser.Id)
                return;

            if (message.Content.StartsWith("!"))
            {
                if (message.Content == "!ping")
                    await message.Channel.SendMessageAsync("pong!");

                else if (message.Content == "!stop" && message.Author.GetDBUser().IsAdmin)
                    await CmdStopAsync();

                else if (message.Content == "!stats")
                    await CmdStatsAsync(message);

                else if (message.Content == "!csv")
                    await CmdCsvAsync(message);

                else
                    await message.Channel.SendMessageAsync("Unrecognized Command");

                await Log.LogAsync(message.Author, $"Recieved: {message.Content}");
            }
        }
    }
}
