using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SpotifyPlaylistMixer.DataObjects
{
    public class Config
    {
        public List<Playlist> SourcePlaylists { get; set; }
        public Playlist TargetPlaylist { get; set; }
        public ObservableCollection<User> Users { get; set; }

        public Config()
        {
            SourcePlaylists = new List<Playlist>();
            TargetPlaylist = new Playlist();
            Users = new ObservableCollection<User>();
        }
    }
}
