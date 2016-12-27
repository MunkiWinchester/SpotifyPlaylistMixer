using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using SpotifyPlaylistMixer.DataObjects;
using static SpotifyPlaylistMixer.Business.Extensions;

namespace SpotifyPlaylistMixer.Business
{
    public static class FileHandler
    {
        public static Config LoadConfig()
        {
            Config config;
            const string filePath = @"N:\EDV\IT-ERP - Intern\ERP Mix der Woche\Config.json";
            if (File.Exists(filePath))
            {
                config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(filePath));
            }
            else
            {
                config = JsonConvert.DeserializeObject<Config>(File.ReadAllText("Config.json"));
                File.Copy("Config.json", filePath);
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
            WriteColoredConsole($"Saving \"{playlistName}\"-playlist-JSON to \"{filePath}\"", ConsoleColor.Magenta);
            var json = JsonConvert.SerializeObject(playlist, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }
    }
}
