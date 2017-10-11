namespace SpotifyPlaylistMixer.DataObjects
{
    public class Playlist
    {
        public string Identifier { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public User Owner { get; set; } = new User();
    }
}