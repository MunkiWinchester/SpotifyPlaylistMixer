using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
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
    }
}