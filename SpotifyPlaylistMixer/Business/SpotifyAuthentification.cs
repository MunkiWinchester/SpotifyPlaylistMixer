using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;
using SpotifyPlaylistMixer.DataObjects;

namespace SpotifyPlaylistMixer.Business
{
    public class SpotifyAuthentification
    {
        private SpotifyWebAPI _spotify;
        
        public async Task<bool> RunAuthentication()
        {
            var webApiFactory = new WebAPIFactory(
                "http://localhost",
                8000,
                "7fa845408d634311aff87a53a3b08f12",
                Scope.UserReadPrivate | Scope.UserReadEmail | Scope.PlaylistReadPrivate | Scope.UserLibraryRead |
                Scope.UserFollowRead | Scope.UserReadBirthdate | Scope.UserTopRead | Scope.PlaylistModifyPublic);

            try
            {
                Extensions.WriteColoredConsole("Get authorazation!", ConsoleColor.White);
                _spotify = await webApiFactory.GetWebApi();
            }
            catch (Exception ex)
            {
                Extensions.WriteColoredConsole(ex.Message, ConsoleColor.Red);
            }

            if (_spotify != null) return true;

            Extensions.WriteColoredConsole("Authorazation failed!", ConsoleColor.Red);
            return false;
        }

        private static void WriteResponse(BasicModel response)
        {
            if (!response.HasError())
                Extensions.WriteColoredConsole("Success", ConsoleColor.Green);
            else
                Extensions.WriteColoredConsole(response.Error.Message, ConsoleColor.Red);
        }

        public IEnumerable<SimplePlaylist> GetPlaylists(string userId)
        {
            var playlists = _spotify.GetUserPlaylists(userId);
            var list = playlists.Items.ToList();
            while (playlists.HasNextPage())
            {
                playlists = _spotify.GetUserPlaylists(userId, playlists.Limit, playlists.Offset + playlists.Limit);
                list.AddRange(playlists.Items);
            }
            return list;
        }

        public void RemovePlaylistTracks(string userId, string playlistId, List<DeleteTrackUri> deleteList)
        {
            WriteResponse(_spotify.RemovePlaylistTracks(userId, playlistId, deleteList));
        }

        public Paging<PlaylistTrack> GetPlaylistTracks(string userId, string playlistId, int limit = 100, int offset = 0)
        {
            return _spotify.GetPlaylistTracks(userId, playlistId, offset: offset, limit: limit);
        }

        public void RemovePlaylistTrack(string userId, string playlistId, DeleteTrackUri deleteTrackUri)
        {
            WriteResponse(_spotify.RemovePlaylistTrack(userId, playlistId, deleteTrackUri));
        }

        public void AddPlaylistTrack(string userId, string playlistId, string songUri)
        {
            WriteResponse(_spotify.AddPlaylistTrack(userId, playlistId, songUri));
        }

        public void AddPlaylistTracks(string userId, string playlistId, List<string> uriList)
        {
            WriteResponse(_spotify.AddPlaylistTracks(userId, playlistId, uriList));
        }

        private SeveralArtists GetSeveralArtists(List<string> ids)
        {
            var artists = _spotify.GetSeveralArtists(ids);
            if (artists.HasError())
            {
                if (artists.Error.Status == 429)
                {
                    Thread.Sleep((int)TimeSpan.FromSeconds(5).TotalMilliseconds);
                    GetSeveralArtists(ids);
                }
            }
            // Don't stress the API
            Thread.Sleep(750);
            return artists;
        }

        public PlaylistElement GetPlaylistElementFromTrack(FullTrack track)
        {
            var playlistElement = new PlaylistElement {Track = track.Name, TrackId = track.Id};

            var ids = track.Artists.Select(a => a.Id).ToList();
            var artists = GetSeveralArtists(ids);

            var artistsList = new List<string>();
            var genresList = new List<string>();
            if (artists.Artists != null)
                foreach (var fullArtist in artists.Artists)
                {
                    artistsList.Add(fullArtist.Name);
                    genresList.AddRange(fullArtist.Genres);
                }
            artistsList.Sort();
            genresList.Sort();
            var artistList = track.Artists.Select(x => x.Name).ToList();
            artistList.Sort();
            playlistElement.Artists = artistsList.Any() ? new CustomList<string>(artistsList) : new CustomList<string>();
            playlistElement.Genres = genresList.Any() ? new CustomList<string>(genresList) : new CustomList<string>();

            return playlistElement;
        }
    }
}