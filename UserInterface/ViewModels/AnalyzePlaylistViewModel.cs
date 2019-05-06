using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Business.Business;
using DataObjects.DataObjects;
using SpotifyAPI.Web.Models;
using WpfUtility.Services;

namespace UserInterface.ViewModels
{
    public class AnalyzePlaylistViewModel : ObservableObject
    {
        private readonly SpotifyAuthentification _spotifyAuthentification;
        private ObservableCollection<PlaylistElement> _originalPlaylist;
        private string _playlistId;
        private string _userId;
        private SimplePlaylist _selectedPlaylist;
        private int _totalItems;
        private List<SimplePlaylist> _playlists;

        public string PlaylistId
        {
            get => _playlistId;
            set
            {
                _playlistId = value;
                OnPropertyChanged();
            }
        }

        public List<SimplePlaylist> Playlists
        {
            get => _playlists;
            set
            {
                _playlists = value;
                OnPropertyChanged();
            }
        }

        public int TotalItems
        {
            get => _totalItems;
            set
            {
                _totalItems = value;
                OnPropertyChanged();
            }
        }

        public SimplePlaylist SelectedPlaylist
        {
            get => _selectedPlaylist;
            set
            {
                _selectedPlaylist = value;
                LoadPlaylist();
                OnPropertyChanged();
            }
        }

        public ObservableCollection<PlaylistElement> SelectedFullPlaylist
        {
            get => _originalPlaylist;
            set
            {
                _originalPlaylist = value;
                OnPropertyChanged();
            }
        }

        public string UserId
        {
            get => _userId;
            set
            {
                _userId = value;
                LoadPlaylists();
                OnPropertyChanged();
            }
        }

        public AnalyzePlaylistViewModel()
        {
            _spotifyAuthentification = new SpotifyAuthentification();
        }

        public void AuthenticateSpotify()
        {
            if (_spotifyAuthentification != null)
            {
                _spotifyAuthentification.RunAuthentication();
            }
        }

        public void LoadPlaylists()
        {
            if (!string.IsNullOrWhiteSpace(_userId))
                Playlists = _spotifyAuthentification.GetPlaylists(_userId).ToList();
        }
        
        public void LoadPlaylist()
        {
            if (_selectedPlaylist != null)
            {
                SelectedFullPlaylist =
                    new ObservableCollection<PlaylistElement>(
                        AddTracksFromPlaylistToPlaylist(_selectedPlaylist.Owner.Id, _selectedPlaylist.Id));
                TotalItems = _originalPlaylist.Count;
            }
        }

        private List<PlaylistElement> AddTracksFromPlaylistToPlaylist(string userIdFrom, string playlistIdFrom)
        {
            var list = new List<PlaylistTrack>();
            var tracks = _spotifyAuthentification.GetPlaylistTracks(userIdFrom, playlistIdFrom);
            list.AddRange(tracks.Items);
            while (tracks.HasNextPage())
            {
                tracks = _spotifyAuthentification.GetPlaylistTracks(userIdFrom, playlistIdFrom, tracks.Limit,
                    tracks.Offset + tracks.Limit);
                list.AddRange(tracks.Items);
            }
            tracks = _spotifyAuthentification.GetPlaylistTracks(userIdFrom, playlistIdFrom, tracks.Limit,
                tracks.Offset + tracks.Limit);
            list.AddRange(tracks.Items);

            var playlistElements = new List<PlaylistElement>();
            foreach (var playlistTrack in list)
            {
                playlistElements.Add(
                    _spotifyAuthentification.GetPlaylistElementFromTrack(playlistTrack.Track));
            }

            return playlistElements;
        }

        public void SearchInCurrentPlaylist()
        {
            var searchTerm = PlaylistId?.ToLower();
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                //SelectedPlaylist = _originalPlaylist;
                return;
            }

            //var converted = _originalPlaylist;
            //var returnPlaylist = new List<PlaylistElement>();
            //returnPlaylist.AddRange(converted.FindAll(x => x.Track.ToLower().Contains(searchTerm)));
            //returnPlaylist.AddRange(converted.FindAll(x => x.User.ToLower().Contains(searchTerm)));
            //returnPlaylist.AddRange(converted.FindAll(x => x.TrackId.Equals(searchTerm)));
            //foreach (var playlistElement in converted)
            //{
            //    returnPlaylist.AddRange(from playlistElementArtist in playlistElement.Artists
            //                            where playlistElementArtist.ToLower().Contains(searchTerm)
            //                            select playlistElement);
            //    returnPlaylist.AddRange(from playlistElementGenre in playlistElement.Genres
            //                            where playlistElementGenre.ToLower().Contains(searchTerm)
            //                            select playlistElement);
            //}
            //SelectedPlaylist = new ObservableCollection<PlaylistElement>(returnPlaylist.Distinct().ToList());
        }
    }
}