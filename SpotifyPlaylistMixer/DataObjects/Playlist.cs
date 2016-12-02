namespace SpotifyPlaylistMixer.DataObjects
{
    public class Playlist
    {
        public string Identifier { get; set; }
        public string Name { get; set; }
        public User Owner { get; set; } 
    }
}