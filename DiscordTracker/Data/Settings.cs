using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordTracker.Data
{
    public class Setting
    {
        public string Id { get; set; }
        public string Value { get; set; }

        public string GetStringValue { get => Value; }
        public int GetIntValue { get => Convert.ToInt32(Value); }
        public bool GetBoolValue { get => Convert.ToBoolean(Value); }

        public static string DiscordToken { get => Program._db.Settings.Find("DISCORD_TOKEN").Value; set { Program._db.Settings.Find("DISCORD_TOKEN").Value = value; Program._db.SaveChanges(); } }
    }
}
