﻿using System;
using System.Timers;
using System.Collections.Generic;
using System.Text;
using DiscordTracker.Data;
using MCServerStatus;

namespace DiscordTracker.Commands
{
    public class Minecraft
    {
        private static Timer _updater = new Timer(30000);
        private static MinecraftPinger pinger;

        public static void Start()
        {
            _updater.Start();
            _updater.Elapsed += _updater_Elapsed;
            _updater_Elapsed(null, null);
        }

        private async static void _updater_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (pinger == null && Setting.McServerAddress != null)
                pinger = new MCServerStatus.MinecraftPinger(Setting.McServerAddress, Convert.ToInt16(Setting.McServerPort));

            if (pinger != null)
            {
                var ping = await pinger.PingAsync();

                var sb = new StringBuilder();
                sb.Append($"{ping.Players.Online}/{ping.Players.Max}");
                foreach (var player in ping.Players.Sample)
                {
                    sb.Append($" {player.Name}");
                }
                await Program._client.SetGameAsync(sb.ToString());
            } else
            {
                await Program._client.SetGameAsync("");
            }
        }
    }
}
