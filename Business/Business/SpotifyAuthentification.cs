﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataObjects.DataObjects;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;

namespace Business.Business
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
                Scope.UserReadPrivate | Scope.PlaylistReadPrivate | Scope.UserLibraryRead |
                Scope.UserFollowRead | Scope.UserTopRead | Scope.PlaylistModifyPublic);

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
            var list = playlists?.Items?.ToList();
            if (list != null)
                while (playlists.HasNextPage())
                {
                    playlists = _spotify.GetUserPlaylists(userId, playlists.Limit, playlists.Offset + playlists.Limit);
                    list.AddRange(playlists.Items);
                }
            else
                list = new List<SimplePlaylist>();
            return list;
        }

        public FullPlaylist GetPlaylist(string userId, string playlistId)
        {
            return _spotify.GetPlaylist(userId, playlistId);
        }

        public Paging<PlaylistTrack> GetPlaylistTracks(string userId, string playlistId, int limit = 100,
            int offset = 0)
        {
            return _spotify.GetPlaylistTracks(userId, playlistId, offset: offset, limit: limit);
        }

        public void RemovePlaylistTrack(string userId, string playlistId, DeleteTrackUri deleteTrackUri)
        {
            WriteResponse(_spotify.RemovePlaylistTrack(userId, playlistId, deleteTrackUri));
        }

        public void RemovePlaylistTracks(string userId, string playlistId, List<DeleteTrackUri> deleteList)
        {
            for (var i = 0; i < deleteList.Count; i = i + 100)
            {
                Extensions.WriteColoredConsole($"Deleting {i} - {i + 100} of {deleteList.Count}", ConsoleColor.White);
                var items = deleteList.Skip(i).Take(100).ToList();
                WriteResponse(_spotify.RemovePlaylistTracks(userId, playlistId, items));
            }
        }

        public void AddPlaylistTrack(string userId, string playlistId, string songUri)
        {
            WriteResponse(_spotify.AddPlaylistTrack(userId, playlistId, songUri));
        }

        public void AddPlaylistTracks(string userId, string playlistId, List<string> uriList)
        {
            for (var i = 0; i < uriList.Count; i = i + 100)
            {
                Extensions.WriteColoredConsole($"Adding {i} - {i + 100} of {uriList.Count}", ConsoleColor.White);
                var items = uriList.Skip(i).Take(100).ToList();
                WriteResponse(_spotify.AddPlaylistTracks(userId, playlistId, items));
            }
        }

        private SeveralArtists GetSeveralArtists(List<string> ids)
        {
            var artists = _spotify.GetSeveralArtists(ids);
            if (artists.HasError())
                if (artists.Error.Status == 429)
                {
                    Thread.Sleep((int) TimeSpan.FromSeconds(5).TotalMilliseconds);
                    GetSeveralArtists(ids);
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
            playlistElement.Artists = artistsList.Any() ? new List<string>(artistsList) : new List<string>();
            playlistElement.Genres = genresList.Any() ? new List<string>(genresList) : new List<string>();

            return playlistElement;
        }
    }
}