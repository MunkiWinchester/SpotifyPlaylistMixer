using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using SpotifyPlaylistMixer.DataObjects;

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

        public static void SavePlaylistAsJson(IEnumerable<KeyValuePair<string, List<string>>> playlist)
        {
            var ci = System.Threading.Thread.CurrentThread.CurrentCulture;
            var fdow = ci.DateTimeFormat.FirstDayOfWeek;
            var today = DateTime.Now.DayOfWeek;
            var sow = DateTime.Now.AddDays(-(today - fdow)).Date;
            var filePath = $@"N:\EDV\IT-ERP - Intern\ERP Mix der Woche\{sow.ToShortDateString().Replace('.', '_')}.json";
            Console.WriteLine($"Saving \"EMP-ERP Mix der Woche\"-playlist-JSON to \"{filePath}\"");
            var json = JsonConvert.SerializeObject(playlist, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }
    }
}
