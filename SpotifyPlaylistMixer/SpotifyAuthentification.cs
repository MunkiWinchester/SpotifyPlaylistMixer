using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using static System.Console;

namespace SpotifyPlaylistMixer
{
    public class SpotifyAuthentification
    {
        private SpotifyWebAPI _spotify;

        private PrivateProfile _profile;
        private readonly List<KeyValuePair<string, string>> _users = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("pc_suchti", "Artur"),
            new KeyValuePair<string, string>("munki666", "Eike"),
            new KeyValuePair<string, string>("1121849657", "Kolja"),
            new KeyValuePair<string, string>("1121873909", "Björn"),
            new KeyValuePair<string, string>("invader_zim85", "Andreas"),
            //new KeyValuePair<string, string>("1129153595", "Philipp")
        };
        private readonly string _erpPlaylist; // EMP-ERP Mix der Woche

        public SpotifyAuthentification()
        {
            _erpPlaylist = "0GNGC39QMDX6lwwOwEac5N";
            RunAuthentication();
        }

        private void InitialSetup()
        {
            WriteLine("Get private profile..");
            _profile = _spotify.GetPrivateProfile();

            //_playlists = GetPlaylists();

            WriteLine("Starting with the \"Mix der Woche\" history!");
            GetMixDerWoche();
        }

        private void GetMixDerWoche()
        {
            RemoveTracksFromPlaylist(_profile.Id, _erpPlaylist);
            foreach (var user in _users)
            {
                WriteLine($"Loading playlists from {user.Value}");
                var playlists = GetPlaylists(user.Key);
                WriteLine($"Loading playlist \"Dein Mix der Woche\" from {user.Value}");
                var playlist = playlists.FirstOrDefault(x => x.Name.Equals("Dein Mix der Woche"));
                // Mix der Woche gehört Spotify
                if (playlist != null)
                {
                    WriteLine($"Begin adding tracks \"Dein Mix der Woche\" from {user.Value} \"EMP-ERP Mix der Woche\"");
                    AddTracksFromPlaylistToPlaylist("spotifydiscover", playlist.Id, _profile.Id, _erpPlaylist);
                }
            }
        }

        private void AddTracksFromPlaylistToPlaylist(string userIdFrom, string playlistIdFrom, string userIdTo,
            string playlistIdTo)
        {
            var tracks = _spotify.GetPlaylistTracks(userIdFrom, playlistIdFrom);
            var uriList = AddTracksToUriList(tracks.Items);
            while (tracks.HasNextPage())
            {
                tracks = _spotify.GetPlaylistTracks(userIdFrom, playlistIdFrom, limit: tracks.Limit, offset: tracks.Offset + tracks.Limit);
                uriList = AddTracksToUriList(tracks.Items);
            }
            var response = _spotify.AddPlaylistTracks(userIdTo, playlistIdTo, uriList);
            WriteResponse(response);
        }

        private static void WriteResponse(ErrorResponse response)
        {
            if (!response.HasError())
                WriteLine("Success");
            else
                WriteLine(response.Error);
        }

        private static List<string> AddTracksToUriList(List<PlaylistTrack> tracks)
        {
            return tracks.Select(playlistTrack => playlistTrack.Track.Uri).ToList();
        }

        private void RemoveTracksFromPlaylist(string userId, string playlistId)
        {
            WriteLine("Loading \"EMP-ERP Mix der Woche\"-Playlist..");
            var erpMix = _spotify.GetPlaylistTracks(userId, playlistId);
            WriteLine($"Gathering and deleting {erpMix.Offset} - {erpMix.Limit} of {erpMix.Total} from \"EMP-ERP Mix der Woche\"");
            RemoveTracksFromPlaylist(erpMix.Items, userId, playlistId);
            while (erpMix.HasNextPage())
            {
                erpMix = _spotify.GetPlaylistTracks(userId, playlistId, limit: erpMix.Limit,
                    offset: erpMix.Offset + erpMix.Limit);
                WriteLine($"Gathering and deleting {erpMix.Offset} - {erpMix.Limit} of {erpMix.Total} from \"EMP-ERP Mix der Woche\"");
                RemoveTracksFromPlaylist(erpMix.Items, userId, playlistId);
            }
        }

        private void RemoveTracksFromPlaylist(List<PlaylistTrack> tracks, string userId,
            string playlistId)
        {
            var deleteList = tracks.Select(track => new DeleteTrackUri(track.Track.Uri)).ToList();
            var response = _spotify.RemovePlaylistTracks(userId, playlistId, deleteList);
            WriteResponse(response);
        }

        private List<SimplePlaylist> GetPlaylists(string userId)
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

        private async void RunAuthentication()
        {
            var webApiFactory = new WebAPIFactory(
                "http://localhost",
                8000,
                "7fa845408d634311aff87a53a3b08f12",
                Scope.UserReadPrivate | Scope.UserReadEmail | Scope.PlaylistReadPrivate | Scope.UserLibraryRead |
                Scope.UserFollowRead | Scope.UserReadBirthdate | Scope.UserTopRead | Scope.PlaylistModifyPublic);

            try
            {
                WriteLine("Get authorazation!");
                _spotify = await webApiFactory.GetWebApi();
            }
            catch (Exception ex)
            {
                WriteLine(ex.Message);
            }

            if (_spotify == null)
                return;

            WriteLine("Initial setup!");
            InitialSetup();
        }
    }
}