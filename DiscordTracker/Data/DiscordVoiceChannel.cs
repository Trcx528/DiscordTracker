namespace DiscordTracker.Data
{
    public class DiscordVoiceChannel
    {
        public ulong Id { get; set; }
        public string Name { get; set; }
        public Discord.WebSocket.SocketChannel VoiceChannel { get => Program._client.GetChannel(this.Id); }
    }
}