using System.Collections.ObjectModel;

namespace SpotifyPlaylistMixer.DataObjects
{
    public class Config
    {
        public Config()
        {
            SourcePlaylists = new ObservableCollection<Playlist>();
            TargetPlaylist = new Playlist();
            Users = new ObservableCollection<User>();
        }

        public ObservableCollection<Playlist> SourcePlaylists { get; set; }
        public Playlist TargetPlaylist { get; set; }
        public ObservableCollection<User> Users { get; set; }
    }
}