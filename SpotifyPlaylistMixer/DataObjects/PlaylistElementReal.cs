using System.Collections.Generic;

namespace SpotifyPlaylistMixer.DataObjects
{
    public class PlaylistElementReal
    {
        public string User { get; set; }
        public List<string> Artists { get; set; }
        public string Track { get; set; }
        public string TrackId { get; set; }
        public List<string> Genres { get; set; }
    }
}