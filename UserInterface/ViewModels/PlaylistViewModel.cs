using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Business.Business;
using DataObjects.DataObjects;
using WpfUtility.Services;

namespace UserInterface.ViewModels
{
    public class PlaylistViewModel : ObservableObject
    {
        private ObservableCollection<KeyValuePair<string, string>> _existingPlaylists;
        private ObservableCollection<PlaylistElement> _originalPlaylist;
        private string _searchTerm;
        private ObservableCollection<PlaylistElement> _selectedPlaylist;
        private string _selectedPlaylistPath;
        private int _totalItems;

        public string SelectedPlaylistPath
        {
            get => _selectedPlaylistPath;
            set
            {
                _selectedPlaylistPath = value;
                LoadExistingPlaylistFromPath(value);
                OnPropertyChanged();
            }
        }

        public string SearchTerm
        {
            get => _searchTerm;
            set
            {
                _searchTerm = value;
                SearchInCurrentPlaylist();
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

        public ObservableCollection<KeyValuePair<string, string>> ExistingPlaylists
        {
            get => _existingPlaylists;
            set => SetField(ref _existingPlaylists, value);
        }

        public ObservableCollection<PlaylistElement> SelectedPlaylist
        {
            get => _selectedPlaylist;
            set => SetField(ref _selectedPlaylist, value);
        }

        public void LoadExistingPlaylistFromPath(string path)
        {
            var playlist = FileHandler.LoadPlaylistElements(path);
            TotalItems = playlist.Count;
            SelectedPlaylist = _originalPlaylist = new ObservableCollection<PlaylistElement>(playlist);
            SearchInCurrentPlaylist();
        }

        public void LoadExistingPlaylistsFromPath(string path)
        {
            ExistingPlaylists =
                new ObservableCollection<KeyValuePair<string, string>>(FileHandler.LoadExistingPlaylistsFromPath(path));
            var first = ExistingPlaylists.First();
            SelectedPlaylistPath = first.Key;
        }

        public void SearchInCurrentPlaylist()
        {
            var searchTerm = SearchTerm;
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                SelectedPlaylist = _originalPlaylist;
                return;
            }

            var converted = _originalPlaylist.ToList();
            var returnPlaylist = new List<PlaylistElement>();
            returnPlaylist.AddRange(converted.FindAll(x => x.Track.ToLower().Contains(searchTerm)));
            returnPlaylist.AddRange(converted.FindAll(x => x.User.ToLower().Contains(searchTerm)));
            returnPlaylist.AddRange(converted.FindAll(x => x.TrackId.Equals(searchTerm)));
            foreach (var playlistElement in converted)
            {
                returnPlaylist.AddRange(from playlistElementArtist in playlistElement.Artists
                    where playlistElementArtist.ToLower().Contains(searchTerm)
                    select playlistElement);
                returnPlaylist.AddRange(from playlistElementGenre in playlistElement.Genres
                    where playlistElementGenre.ToLower().Contains(searchTerm)
                    select playlistElement);
            }
            SelectedPlaylist = new ObservableCollection<PlaylistElement>(returnPlaylist.Distinct().ToList());
        }
    }
}