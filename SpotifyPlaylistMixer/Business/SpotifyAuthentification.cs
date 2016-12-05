using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;

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
                Console.WriteLine("Get authorazation!");
                _spotify = await webApiFactory.GetWebApi();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            if (_spotify != null) return true;

            Console.WriteLine("Authorazation failed!");
            return false;
        }

        private static void WriteResponse(BasicModel response)
        {
            if (!response.HasError())
                Console.WriteLine("Success");
            else
                Console.WriteLine(response.Error);
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
    }
}