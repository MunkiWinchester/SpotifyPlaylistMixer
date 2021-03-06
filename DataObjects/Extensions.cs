﻿using System.Collections.Generic;
using System.Linq;
using DataObjects.DataObjects;

namespace DataObjects
{
    public static class Extensions
    {
        public static string ToSeperatedString(this List<Playlist> playlists)
        {
            var seperatedString = string.Empty;
            foreach (var playlist in playlists)
                seperatedString = $"{seperatedString} or \"{playlist.Name} (Owner: {playlist.Owner.Name})\"";
            seperatedString = seperatedString.TrimStart(' ', 'o', 'r');
            return seperatedString;
        }

        public static string ToConnectedString<T>(this List<T> value)
        {
            var list = value as List<string>;
            var connectedString = "";
            if (list != null && list.Any())
                connectedString = list.Aggregate((s, next) => $"{s}; {next}");
            return connectedString;
        }
    }
}