using System.Collections.Generic;

namespace SpotifyPlaylistMixer.DataObjects
{
    public class Config
    {
        public List<Playlist> SourcePlaylists { get; set; }
        public Playlist TargetPlaylist { get; set; }
        public List<User> Users { get; set; }
    }
}
