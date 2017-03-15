﻿using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using SpotifyPlaylistMixer.DataObjects;

namespace SpotifyPlaylistMixer.Business
{
    public static class FileHandler
    {
        public static Config LoadConfig(string path)
        {
            var config = new Config();
            if (File.Exists(path))
            {
                config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(path));
            }
            return config;
        }

        public static void SavePlaylistAsJson(string playlistName, IEnumerable<KeyValuePair<string, List<string>>> playlist)
        {
            var ci = System.Threading.Thread.CurrentThread.CurrentCulture;
            var fdow = ci.DateTimeFormat.FirstDayOfWeek;
            var today = DateTime.Now.DayOfWeek;
            var sow = DateTime.Now.AddDays(-(today - fdow)).Date;
            var filePath = $@"N:\EDV\IT-ERP - Intern\ERP Mix der Woche\{sow.ToShortDateString().Replace('.', '_')}.json";
            Extensions.WriteColoredConsole($"Saving \"{playlistName}\"-playlist-JSON to \"{filePath}\"", ConsoleColor.Magenta);
            var json = JsonConvert.SerializeObject(playlist, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }
    }
}
