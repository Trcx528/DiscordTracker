using DiscordTracker.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordTracker
{
    public static class Extras
    {
        public static DiscordUser GetDBUser(this Discord.WebSocket.SocketUser sUser) => Program._db.DiscordUser.Find(sUser.Id);
    }
}
