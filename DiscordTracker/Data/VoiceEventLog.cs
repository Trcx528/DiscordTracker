﻿using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordTracker.Data
{
    public class VoiceEventLog
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public ulong UserId { get; set; }
        public string EventType { get; set; }
        public ulong ChannelId { get; set; }

        public DiscordVoiceChannel Channel { get; set; }
        public DiscordUser User { get; set; }


        public async static Task Log(SocketUser user, SocketChannel chan, string eventName)
        {
            var u = await DiscordUser.CreateOrGetAsync(user);
            var c = await DiscordVoiceChannel.CreateOrGetAsync(chan);

            var vel = new VoiceEventLog() { Date = DateTime.Now, ChannelId = c.Id, UserId = u.Id, EventType = eventName };
            var db = new ApplicationDataContext();
            db.Add(vel);
            await db.SaveChangesAsync();
            db.Dispose();
        }
    }
}