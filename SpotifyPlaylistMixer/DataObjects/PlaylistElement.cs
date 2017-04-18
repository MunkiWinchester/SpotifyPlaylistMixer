namespace SpotifyPlaylistMixer.DataObjects
{
    public class PlaylistElement
    {
        public string User { get; set; }
        public CustomList<string> Artists { get; set; }
        public string Track { get; set; }
        public string TrackId { get; set; }
        public CustomList<string> Genres { get; set; }
    }
}