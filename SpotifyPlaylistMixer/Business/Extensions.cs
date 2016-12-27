﻿using System;
using System.Collections.Generic;
using SpotifyPlaylistMixer.DataObjects;

namespace SpotifyPlaylistMixer.Business
{
    public static class Extensions
    {
        public static string ToSeperatedString(this List<Playlist> playlists)
        {
            var seperatedString = string.Empty;
            foreach (var playlist in playlists)
            {
                seperatedString = $"{seperatedString} or \"{playlist.Name} (Owner: {playlist.Owner.Name})\"";
            }
            seperatedString = seperatedString.TrimStart(' ', 'o', 'r');
            return seperatedString;
        }

        public static void WriteColoredConsole(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Logger.Trace(text);
            Console.WriteLine(text);
        }
    }
}