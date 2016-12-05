using System.Collections.Generic;

namespace SpotifyPlaylistMixer.DataObjects
{
    public class Config
    {
        public Playlist SourcePlaylist { get; set; }
        public Playlist TargetPlaylist { get; set; }
        public List<User> Users { get; set; }
    }
}
