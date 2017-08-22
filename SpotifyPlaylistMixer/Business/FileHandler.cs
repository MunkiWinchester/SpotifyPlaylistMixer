using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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
                try
                {
                    config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(path));
                }
                catch (Exception ex)
                {
                    Logger.Error("LoadConfig failed", ex);
                    return null;
                }
            }
            return config;
        }

        public static void SavePlaylistAsJson(string playlistName, IEnumerable<PlaylistElement> playlist)
        {
            var ci = System.Threading.Thread.CurrentThread.CurrentCulture;
            var fdow = ci.DateTimeFormat.FirstDayOfWeek;
            var today = DateTime.Now.DayOfWeek;
            var sow = DateTime.Now.AddDays(-(today - fdow)).Date;
            var filePath =
                $@"{Directory.GetCurrentDirectory()}\Resources\Examples\{CleanFileName(playlistName)}_{sow.ToShortDateString()
                    .Replace('.', '_')}.json";
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

        public static User SaveConfigAddUser(List<KeyValuePair<string, string>> path)
        {
            //Muss leider bestehen bleiben weil der Path unerwartet geändert werden könnte
            //TODO: Meldung noch anpassen
            var config = new Config();
            if (File.Exists(path.First().Key))
            {
                try
                {
                    config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(path.First().Key));
                }
                catch (Exception ex)
                {
                    Logger.Error("SaveConfigAddUser failed", ex);
                }
            }

            var user = new User("", "");

            config.Users.Add(user);
            File.WriteAllText(path.First().Key, JsonConvert.SerializeObject(config));
            return user;
        }

        /*evtl. zu SaveConfigEditUser zusammen führen */
        public static User SaveConfigDeleteUser(List<string> path, User user)
        {
            //Muss leider bestehen bleiben weil der Path unerwartet geändert werden könnte
            //TODO: Meldung noch anpassen
            var config = new Config();
            if (File.Exists(path.First()))
            {
                try
                {
                    config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(path.First()));
                }
                catch (Exception ex)
                {
                    Logger.Error("SaveConfigDeleteUser failed", ex);
                }
            }

            config.Users.Remove(user);
            File.WriteAllText(path.First(), JsonConvert.SerializeObject(config));
            return user;
        }

        public static void SaveConfigEditUser(List<KeyValuePair<string, string>> path, ObservableCollection<User> users)
        {
            var config = new Config();
            if (File.Exists(path.First().Key))
            {
                try
                {
                    config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(path.First().Key));
                }
                catch (Exception ex)
                {
                    Logger.Error("SaveConfigEditUser failed", ex);
                }
            }
            for (var i = 0; i < users.Count; i++)
            {
                config.Users[i] = !config.Users[i].Equals(users[i]) ? users[i] : config.Users[i];
            }

            File.WriteAllText(path.First().Key, JsonConvert.SerializeObject(config));
        }
    }
}
