using DiscordTracker.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordTracker
{
    public static class Extras
    {
        private static ApplicationDataContext _db = new ApplicationDataContext();
        public static DiscordUser GetDBUser(this Discord.WebSocket.SocketUser sUser) => _db.DiscordUser.Find(sUser.Id);
        public static bool IsAdmin(this Discord.WebSocket.SocketUser sUser) => sUser.GetDBUser().IsAdmin;
    }
}
