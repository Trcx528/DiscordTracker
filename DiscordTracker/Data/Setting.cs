using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordTracker.Data
{
    public class Setting
    {
        private static ApplicationDataContext db = new ApplicationDataContext();
        public string Id { get; set; }
        public string Value { get; set; }

        public int ToInt { get => Convert.ToInt32(Value); }
        public bool ToBool { get => Convert.ToBoolean(Value); }

        public static string DiscordToken { get => db.Settings.Find("DISCORD_TOKEN").Value; set { db.Settings.Find("DISCORD_TOKEN").Value = value; db.SaveChanges(); } }
        public static string McServerAddress { get => db.Settings.Find("MC_SERVER").Value; set { db.Settings.Find("MC_SERVER").Value = value; db.SaveChanges(); } }
        public static int McServerPort { get => db.Settings.Find("MC_SERVER_PORT").ToInt; set { db.Settings.Find("MC_SERVER").Value = value.ToString(); db.SaveChanges(); } }
    }
}
