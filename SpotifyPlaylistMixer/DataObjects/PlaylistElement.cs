using System.Collections.Generic;

namespace SpotifyPlaylistMixer.DataObjects
{
    public class PlaylistElement
    {
        public string User { get; set; }
        public List<string> Artists { get; set; }
        public string Track { get; set; }
        public List<string> Genres { get; set; }
    }
}