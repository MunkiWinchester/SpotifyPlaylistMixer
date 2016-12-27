using System;
using SpotifyPlaylistMixer.Business;

namespace SpotifyPlaylistMixer
{
    internal class Program
    {
        private static void Main()
        {
            var spotifyAuthentification = new SpotifyAuthentification();
            var authenticate = spotifyAuthentification.RunAuthentication();
            authenticate.Wait();
            if (authenticate.Result)
            {
                var playlistHandler = new PlaylistHandler(spotifyAuthentification);
                playlistHandler.CreateMixDerWoche();
            }
            Console.ReadKey();
        }
    }
}