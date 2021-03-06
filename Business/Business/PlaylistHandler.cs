﻿using System;
using System.Collections.Generic;
using System.Linq;
using DataObjects.DataObjects;
using SpotifyAPI.Web.Models;

namespace Business.Business
{
    public class PlaylistHandler
    {
        private readonly List<PlaylistElement> _personPlaylistElements = new List<PlaylistElement>();
        private readonly SpotifyAuthentification _spotifyAuthentification;
        private Config _config;

        public PlaylistHandler(SpotifyAuthentification spotifyAuthentification)
        {
            _spotifyAuthentification = spotifyAuthentification;
        }

        public void CreateMixDerWoche(Config config)
        {
            _config = config;
            Extensions.WriteColoredConsole(
                $"Starting with the \"{_config.SourcePlaylists.ToList().ToSeperatedString()}\" history!",
                ConsoleColor.White);
            RemoveTracksFromPlaylist(_config.TargetPlaylist.Owner.Identifier, _config.TargetPlaylist.Identifier);
            foreach (var user in _config.Users)
            {
                var foundPlaylist =
                    _config.SourcePlaylists.FirstOrDefault(x => x.Owner.Identifier.Equals(user.Identifier));
                if (foundPlaylist != null)
                {
                    AddTracksFromPlaylistToPlaylis(user,
                        new SimplePlaylist
                        {
                            Id = foundPlaylist.Identifier,
                            Name = foundPlaylist.Name,
                            Owner = new PublicProfile {Id = foundPlaylist.Owner.Identifier}
                        });
                }
                else
                {
                    var hit = GetSimplePlaylistFromUser(user);
                    // Mix der Woche gehört Spotify
                    if (hit != null)
                        AddTracksFromPlaylistToPlaylis(user, hit);
                }
            }

            FileHandler.SavePlaylistAsJson(_config.TargetPlaylist.Name, _personPlaylistElements);

            //RemovingDuplicates(_profile.Id, _erpPlaylist);
        }

        private void AddTracksFromPlaylistToPlaylis(User user, SimplePlaylist playlist)
        {
            Extensions.WriteColoredConsole(
                $"Begin adding tracks from \"{user.Name}\"s \"{playlist.Name}\" to \"{_config.TargetPlaylist.Name}\"",
                ConsoleColor.Cyan);
            AddTracksFromPlaylistToPlaylist(playlist.Owner.Id, playlist.Id,
                _config.TargetPlaylist.Owner.Identifier, _config.TargetPlaylist.Identifier, user.Name);
        }

        private SimplePlaylist GetSimplePlaylistFromUser(User user)
        {
            Extensions.WriteColoredConsole($"Loading playlists from \"{user.Name}\"", ConsoleColor.White);
            var playlists = _spotifyAuthentification.GetPlaylists(user.Identifier).ToList();
            Extensions.WriteColoredConsole(
                $"Loading playlist {_config.SourcePlaylists.ToList().ToSeperatedString()} from \"{user.Name}\"",
                ConsoleColor.White);
            SimplePlaylist hit = null;
            foreach (var configSourcePlaylist in _config.SourcePlaylists)
            foreach (var simplePlaylist in playlists)
            {
                if (!simplePlaylist.Name.Equals(configSourcePlaylist.Name)) continue;
                hit = simplePlaylist;
                break;
            }
            return hit;
        }

        // ReSharper disable once UnusedMember.Local
        private void RemovingDuplicates(string userId, string playlistId)
        {
            Extensions.WriteColoredConsole($"Removing duplicates from \"{_config.TargetPlaylist.Name}\"",
                ConsoleColor.Blue);
            Extensions.WriteColoredConsole($"Loading \"{_config.TargetPlaylist.Name}\"-playlist tracks..",
                ConsoleColor.White);
            var paging = _spotifyAuthentification.GetPlaylistTracks(userId, playlistId);
            var playlistTracks = paging.Items;
            var total = paging.Total;
            Extensions.WriteColoredConsole(
                $"Gathering {paging.Offset} - {paging.Offset + paging.Limit} of {total} from \"{_config.TargetPlaylist.Name}\"",
                ConsoleColor.White);
            while (paging.HasNextPage())
            {
                paging.Offset = paging.Offset + paging.Limit;
                Extensions.WriteColoredConsole(
                    $"Gathering {paging.Offset} - {paging.Offset + paging.Limit} of {total} from \"{_config.TargetPlaylist.Name}\"",
                    ConsoleColor.White);
                paging = _spotifyAuthentification.GetPlaylistTracks(userId, playlistId, paging.Offset);
                playlistTracks.AddRange(paging.Items);
            }
            Extensions.WriteColoredConsole("Finding duplicates", ConsoleColor.Blue);
            var duplicates = playlistTracks
                .GroupBy(x => x.Track.Id)
                .Where(g => g.Count() > 1)
                .Select(y => new {Element = y.Key, Counter = y.Count()});
            foreach (var duplicate in duplicates)
            {
                var track = playlistTracks.FirstOrDefault(x => x.Track.Id.Equals(duplicate.Element));
                if (track != null)
                {
                    var artists =
                        track.Track.Artists.Aggregate(string.Empty,
                                (current, simpleArtist) => current + $"{simpleArtist.Name}, ")
                            .TrimEnd(' ', ',');
                    Extensions.WriteColoredConsole(
                        $"Removing all \"{artists} --- {track.Track.Name}\" (was {duplicate.Counter} times in the playlist)",
                        ConsoleColor.Red);
                    _spotifyAuthentification.RemovePlaylistTrack(userId, playlistId,
                        new DeleteTrackUri(track.Track.Uri));
                    Extensions.WriteColoredConsole($"Readding one \"{artists} --- {track.Track.Name}\"",
                        ConsoleColor.Yellow);
                    _spotifyAuthentification.AddPlaylistTrack(userId, playlistId, track.Track.Uri);
                }
            }
        }

        private void RemoveTracksFromPlaylist(IEnumerable<PlaylistTrack> tracks, string userId,
            string playlistId)
        {
            var deleteList = tracks.Select(track => new DeleteTrackUri(track.Track.Uri)).ToList();
            _spotifyAuthentification.RemovePlaylistTracks(userId, playlistId, deleteList);
        }

        private void RemoveTracksFromPlaylist(string userId, string playlistId)
        {
            Extensions.WriteColoredConsole($"Loading \"{_config.TargetPlaylist.Name}\"-Playlist..", ConsoleColor.White);
            var erpMix = _spotifyAuthentification.GetPlaylistTracks(userId, playlistId);
            var total = erpMix.Total;
            var offset = erpMix.Offset;
            Extensions.WriteColoredConsole(
                $"Gathering and deleting {offset} - {offset + erpMix.Limit} of {total} from \"{_config.TargetPlaylist.Name}\"",
                ConsoleColor.Red);
            RemoveTracksFromPlaylist(erpMix.Items, userId, playlistId);
            while (erpMix.HasNextPage())
            {
                erpMix = _spotifyAuthentification.GetPlaylistTracks(userId, playlistId);
                offset = offset + erpMix.Limit;
                Extensions.WriteColoredConsole(
                    $"Gathering and deleting {offset} - {offset + erpMix.Limit} of {total} from \"{_config.TargetPlaylist.Name}\"",
                    ConsoleColor.Red);
                RemoveTracksFromPlaylist(erpMix.Items, userId, playlistId);
            }
        }

        private IEnumerable<string> AddTracksToUriList(IEnumerable<PlaylistTrack> tracks, string user)
        {
            var uriList = new List<string>();
            if (tracks != null)
            {
                foreach (var playlistTrack in tracks)
                {
                    uriList.Add(playlistTrack.Track.Uri);

                    var playlistElement = _spotifyAuthentification.GetPlaylistElementFromTrack(playlistTrack.Track);
                    playlistElement.User = user;
                    _personPlaylistElements.Add(playlistElement);
                }
                return uriList;
            }
            return new List<string>();
        }

        private void AddTracksFromPlaylistToPlaylist(string userIdFrom, string playlistIdFrom, string userIdTo,
            string playlistIdTo, string user)
        {
            var tracks = _spotifyAuthentification.GetPlaylistTracks(userIdFrom, playlistIdFrom);
            var uriList = AddTracksToUriList(tracks.Items, user).ToList();
            while (tracks.HasNextPage())
            {
                tracks = _spotifyAuthentification.GetPlaylistTracks(userIdFrom, playlistIdFrom, tracks.Limit,
                    tracks.Offset + tracks.Limit);
                uriList.AddRange(AddTracksToUriList(tracks.Items, user));
            }
            tracks = _spotifyAuthentification.GetPlaylistTracks(userIdFrom, playlistIdFrom, tracks.Limit,
                tracks.Offset + tracks.Limit);
            uriList.AddRange(AddTracksToUriList(tracks.Items, user));
            _spotifyAuthentification.AddPlaylistTracks(userIdTo, playlistIdTo, uriList);
        }
    }
}