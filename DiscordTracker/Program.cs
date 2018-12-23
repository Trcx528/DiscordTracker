using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using DiscordTracker.Data;
using System.Linq;
using System.Threading;

namespace DiscordTracker
{
    class Program
    {
        private static readonly DiscordSocketClient _client = new DiscordSocketClient();
        public static readonly ApplicationDataContext _db = new ApplicationDataContext();
        private readonly CancellationTokenSource MainThread = new CancellationTokenSource();

        static void Main(string[] args)
        {
            new Program().MainAsync().GetAwaiter().GetResult();
        }

        public Program()
        {
            _client.Log += LogAsync;
            _client.Ready += ReadyAsync;
            _client.MessageReceived += MessageReceievedAsync;
            _client.UserVoiceStateUpdated += UserVoiceStateUpdatedAsync;
        }

        private async Task UserVoiceStateUpdatedAsync(SocketUser user, SocketVoiceState prevState, SocketVoiceState newState)
        {
            if (prevState.VoiceChannel == null)
            {
                await CallLog.JoinedAsync(user, newState.VoiceChannel.Name);
                Console.WriteLine($"{user.Username} Joined {newState.VoiceChannel.Name}");
            }
            else
            {
                await CallLog.LeftAsync(user, prevState.VoiceChannel.Name);
                Console.WriteLine($"{user.Username} Left {prevState.VoiceChannel.Name}");
            }
        }
        

        public async Task MainAsync()
        {
            // The "right" way to do it lol
            //await _client.LoginAsync(Discord.TokenType.Bot, Environment.GetEnvironmentVariable("DISCORD_TOKEN"));
            await _client.LoginAsync(Discord.TokenType.Bot, "NTI2Mjk0NTM2MzAyMTAwNTAx.DwDFwg.H_SDlodqKTNxmPaEkpiFLGLmvLQ");
            await _client.StartAsync();
            await Task.Delay(-1, MainThread.Token);
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }

        private async Task ReadyAsync()
        {
            Console.WriteLine($"{_client.CurrentUser} connected!");

            var chan = _client.GetChannel(239916716283527169);
            foreach (var u in chan.Users)
            {
                await CallLog.JoinedAsync(u, "General");
            }
        }

        private async Task CmdStopAsync()
        {
            var chan = _client.GetChannel(239916716283527169);
            foreach (var u in chan.Users)
            {
                await CallLog.LeftAsync(u, "General");
            }
            _client.Dispose();
            MainThread.Cancel();
            Environment.Exit(0);
        }

        private async Task MessageReceievedAsync(SocketMessage message)
        {
            if (message.Author.Id == _client.CurrentUser.Id)
                return;

            if (message.Content == "!ping")
                await message.Channel.SendMessageAsync("pong!");

            if (message.Content == "!stop" && message.Author.Username == "Trcx")
                await CmdStopAsync();
        }
    }
}
