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
        private FullPlaylist _originalPlaylist;
        private string _playlistId;
        private string _userId;
        private FullPlaylist _selectedPlaylist;
        private int _totalItems;

        public string PlaylistId
        {
            get => _playlistId;
            set
            {
                _playlistId = value;
                LoadPlaylist();
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

        public FullPlaylist SelectedPlaylist
        {
            get => _selectedPlaylist;
            set => SetField(ref _selectedPlaylist, value);
        }

        public string UserId
        {
            get => _userId;
            set
            {
                _userId = value;
                LoadPlaylist();
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
                var authenticate = _spotifyAuthentification.RunAuthentication();
                authenticate.Wait();
                if (!authenticate.Result)
                {
                    var result = MessageBox.Show(@"Couldn't connect to the spotify api. Retry?", @"Spotify",
                        MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        AuthenticateSpotify();
                    }
                }
            }
        }

        public void LoadPlaylist()
        {
            if (!string.IsNullOrWhiteSpace(_userId) && !string.IsNullOrWhiteSpace(_playlistId))
            {
                SelectedPlaylist = _spotifyAuthentification.GetPlaylist(_userId, _playlistId);
            }
        }

        public void SearchInCurrentPlaylist()
        {
            var searchTerm = PlaylistId?.ToLower();
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                SelectedPlaylist = _originalPlaylist;
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