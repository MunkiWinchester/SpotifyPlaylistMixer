using System.Collections.Generic;
using SpotifyPlaylistMixer.Business;

namespace SpotifyPlaylistMixer.DataObjects
{
    public class PlaylistElement
    {
        public string User { get; set; }
        public List<string> Artists { get; set; }
        public string ArtistsString => Artists.ToConnectedString();
        public string Track { get; set; }
        public string TrackId { get; set; }
        public List<string> Genres { get; set; }
        public string GenresString => Genres.ToConnectedString();
    }
}