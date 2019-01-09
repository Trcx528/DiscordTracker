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

        public string GetStringValue { get => Value; }
        public int GetIntValue { get => Convert.ToInt32(Value); }
        public bool GetBoolValue { get => Convert.ToBoolean(Value); }

        public static string DiscordToken { get => db.Settings.Find("DISCORD_TOKEN").Value; set { db.Settings.Find("DISCORD_TOKEN").Value = value; db.SaveChanges(); } }
    }
}
