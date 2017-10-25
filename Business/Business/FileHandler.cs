using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using DataObjects.DataObjects;
using Newtonsoft.Json;

namespace Business.Business
{
    public static class FileHandler
    {
        public static Config LoadConfig(string path)
        {
            var config = new Config();
            if (File.Exists(path))
                try
                {
                    config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(path));
                }
                catch (Exception ex)
                {
                    Logger.Error("LoadConfig failed", ex);
                    return null;
                }
            return config;
        }

        public static void SavePlaylistAsJson(string playlistName, IEnumerable<PlaylistElement> playlist)
        {
            var ci = Thread.CurrentThread.CurrentCulture;
            var fdow = ci.DateTimeFormat.FirstDayOfWeek;
            var today = DateTime.Now.DayOfWeek;
            var sow = DateTime.Now.AddDays(-(today - fdow)).Date;
            var filePath =
                $@"{Directory.GetCurrentDirectory()}\Resources\Examples\{CleanFileName(playlistName)}_{
                        sow.ToShortDateString()
                            .Replace('.', '_')
                    }.json";
            Extensions.WriteColoredConsole($"Saving \"{playlistName}\"-playlist-JSON to \"{filePath}\"",
                ConsoleColor.Magenta);
            var json = JsonConvert.SerializeObject(playlist, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        private static string CleanFileName(string fileName)
        {
            return Path.GetInvalidFileNameChars()
                .Aggregate(fileName, (current, c) => current.Replace(c.ToString(), string.Empty));
        }

        public static List<KeyValuePair<string, string>> LoadExistingPlaylistsFromPath(string path)
        {
            if (Directory.Exists(path))
            {
                var info = new DirectoryInfo(path);
                var files =
                    info.GetFiles("*.json", SearchOption.TopDirectoryOnly)
                        .OrderByDescending(x => x.LastWriteTime)
                        .Select(x => x.FullName)
                        .ToList();
                return files.Select(file => new KeyValuePair<string, string>(file,
                        file.Substring(file.LastIndexOf("\\", StringComparison.Ordinal) + 1)))
                    .ToList();
            }
            return new List<KeyValuePair<string, string>>();
        }

        public static void SaveConfig(Config config, string file, string path = null)
        {
            if (!string.IsNullOrWhiteSpace(path))
                file = CheckPath(path, file);

            var json = JsonConvert.SerializeObject(config, Formatting.Indented);
            File.WriteAllText(file, json);
        }

        private static string CheckPath(string path, string file)
        {
            var correctedPath = path.EndsWith("\\") ? path : path + "\\";
            var correctedFile = file.EndsWith(".json") ? correctedPath + file : correctedPath + file + ".json";
            using (File.Create(correctedFile))
            {
            }
            return correctedFile;
        }

        public static List<PlaylistElement> LoadPlaylistElements(string path)
        {
            return JsonConvert.DeserializeObject<List<PlaylistElement>>(
                File.ReadAllText(path));
        }
    }
}