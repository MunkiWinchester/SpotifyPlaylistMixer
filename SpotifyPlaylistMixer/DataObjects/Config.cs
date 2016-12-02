using System.Collections.Generic;

namespace SpotifyPlaylistMixer.DataObjects
{
    class Config
    {
        public Playlist SourcePlaylist { get; set; }
        public Playlist TargetPlaylist { get; set; }
        public List<User> Users { get; set; }
    }
}
