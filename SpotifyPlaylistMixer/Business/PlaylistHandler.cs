using System;
using System.Collections.Generic;
using System.Linq;
using SpotifyAPI.Web.Models;
using SpotifyPlaylistMixer.DataObjects;

namespace SpotifyPlaylistMixer.Business
{
    public class PlaylistHandler
    {
        private readonly Config _config;
        private readonly SpotifyAuthentification _spotifyAuthentification;
        private readonly List<KeyValuePair<string, List<string>>> _personSongs = new List<KeyValuePair<string, List<string>>>();

        public PlaylistHandler(SpotifyAuthentification spotifyAuthentification)
        {
            _config = FileHandler.LoadConfig();
            _spotifyAuthentification = spotifyAuthentification;
        }

        public void CreateMixDerWoche()
        {
            Console.WriteLine("Starting with the \"Mix der Woche\" history!");
            RemoveTracksFromPlaylist(_config.TargetPlaylist.Owner.Identifier, _config.TargetPlaylist.Identifier);
            foreach (var user in _config.Users)
            {
                Console.WriteLine($"Loading playlists from {user.Name}");
                var playlists = _spotifyAuthentification.GetPlaylists(user.Identifier);
                Console.WriteLine($"Loading playlist \"Dein Mix der Woche\" from {user.Name}");
                var playlist = playlists.FirstOrDefault(x => x.Name.Equals(_config.SourcePlaylist.Name));
                // Mix der Woche gehört Spotify
                if (playlist != null)
                {
                    Console.WriteLine($"Begin adding tracks \"Dein Mix der Woche\" from {user.Name} \"EMP-ERP Mix der Woche\"");
                    AddTracksFromPlaylistToPlaylist(_config.SourcePlaylist.Owner.Identifier, playlist.Id,
                        _config.TargetPlaylist.Owner.Identifier, _config.TargetPlaylist.Identifier, user.Name);
                }
            }

            FileHandler.SavePlaylistAsJson(_personSongs);

            //RemovingDuplicates(_profile.Id, _erpPlaylist);
        }

        // ReSharper disable once UnusedMember.Local
        private void RemovingDuplicates(string userId, string playlistId)
        {
            Console.WriteLine("Removing duplicates from \"EMP-ERP Mix der Woche\"");
            Console.WriteLine("Loading \"EMP-ERP Mix der Woche\"-playlist tracks..");
            var paging = _spotifyAuthentification.GetPlaylistTracks(userId, playlistId);
            var playlistTracks = paging.Items;
            var total = paging.Total;
            Console.WriteLine($"Gathering {paging.Offset} - {paging.Offset + paging.Limit} of {total} from \"EMP-ERP Mix der Woche\"");
            while (paging.HasNextPage())
            {
                paging.Offset = paging.Offset + paging.Limit;
                Console.WriteLine($"Gathering {paging.Offset} - {paging.Offset + paging.Limit} of {total} from \"EMP-ERP Mix der Woche\"");
                paging = _spotifyAuthentification.GetPlaylistTracks(userId, playlistId, paging.Offset);
                playlistTracks.AddRange(paging.Items);
            }
            Console.WriteLine("Finding duplicates");
            var duplicates = playlistTracks
                .GroupBy(x => x.Track.Id)
                .Where(g => g.Count() > 1)
                .Select(y => new { Element = y.Key, Counter = y.Count() });
            foreach (var duplicate in duplicates)
            {
                var track = playlistTracks.FirstOrDefault(x => x.Track.Id.Equals(duplicate.Element));
                if (track != null)
                {
                    var artists =
                        track.Track.Artists.Aggregate(string.Empty,
                            (current, simpleArtist) => current + $"{simpleArtist.Name}, ").TrimEnd(' ', ',');
                    Console.WriteLine($"Removing all \"{artists} --- {track.Track.Name}\" (was {duplicate.Counter} times in the playlist)");
                    _spotifyAuthentification.RemovePlaylistTrack(userId, playlistId, new DeleteTrackUri(track.Track.Uri));
                    Console.WriteLine($"Reading one time \"{artists} --- {track.Track.Name}\"");
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
            Console.WriteLine("Loading \"EMP-ERP Mix der Woche\"-Playlist..");
            var erpMix = _spotifyAuthentification.GetPlaylistTracks(userId, playlistId);
            var total = erpMix.Total;
            var offset = erpMix.Offset;
            Console.WriteLine($"Gathering and deleting {offset} - {offset + erpMix.Limit} of {total} from \"EMP-ERP Mix der Woche\"");
            RemoveTracksFromPlaylist(erpMix.Items, userId, playlistId);
            while (erpMix.HasNextPage())
            {
                erpMix = _spotifyAuthentification.GetPlaylistTracks(userId, playlistId);
                offset = offset + erpMix.Limit;
                Console.WriteLine($"Gathering and deleting {offset} - {offset + erpMix.Limit} of {total} from \"EMP-ERP Mix der Woche\"");
                RemoveTracksFromPlaylist(erpMix.Items, userId, playlistId);
            }
        }

        private IEnumerable<string> AddTracksToUriList(IEnumerable<PlaylistTrack> tracks, string user)
        {
            var uriList = new List<string>();
            var songList = new List<string>();
            foreach (var playlistTrack in tracks)
            {
                uriList.Add(playlistTrack.Track.Uri);
                var artists =
                    playlistTrack.Track.Artists.Aggregate(string.Empty,
                        (current, simpleArtist) => current + $"{simpleArtist.Name}, ").TrimEnd(' ', ',');
                songList.Add($"{artists} --- {playlistTrack.Track.Name}");
            }
            if (songList.Any())
                _personSongs.Add(new KeyValuePair<string, List<string>>($"{user}", songList));
            return uriList;
        }

        private void AddTracksFromPlaylistToPlaylist(string userIdFrom, string playlistIdFrom, string userIdTo,
            string playlistIdTo, string user)
        {
            var tracks = _spotifyAuthentification.GetPlaylistTracks(userIdFrom, playlistIdFrom);
            var uriList = AddTracksToUriList(tracks.Items, user).ToList();
            while (tracks.HasNextPage())
            {
                tracks = _spotifyAuthentification.GetPlaylistTracks(userIdFrom, playlistIdFrom, tracks.Limit, tracks.Offset + tracks.Limit);
                uriList.AddRange(AddTracksToUriList(tracks.Items, user));
            }
            tracks = _spotifyAuthentification.GetPlaylistTracks(userIdFrom, playlistIdFrom, tracks.Limit, tracks.Offset + tracks.Limit);
            uriList.AddRange(AddTracksToUriList(tracks.Items, user));
            _spotifyAuthentification.AddPlaylistTracks(userIdTo, playlistIdTo, uriList);
        }
    }
}
