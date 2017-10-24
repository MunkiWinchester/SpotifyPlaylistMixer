using System;
using System.Collections.Generic;
using Business.Business;
using DataObjects.DataObjects;

namespace Business
{
    public static class Extensions
    {
        public static string ToSeperatedString(this List<Playlist> playlists)
        {
            return DataObjects.Extensions.ToSeperatedString(playlists);
        }

        public static void WriteColoredConsole(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Logger.Trace(text);
            Console.WriteLine(text);
        }
    }
}