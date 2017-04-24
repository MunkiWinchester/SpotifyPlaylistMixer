using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ReactiveUI;
using SpotifyPlaylistMixer.Business;
using SpotifyPlaylistMixer.DataObjects;

namespace SpotifyPlaylistMixer.ViewModels
{
    public class PlaylistViewModel : ReactiveObject
    {
        private string _path;
        public string Path
        {
            get => _path;
            set => this.RaiseAndSetIfChanged(ref _path, value);
        }

        private string _selectedPlaylistPath;
        public string SelectedPlaylistPath
        {
            get => _selectedPlaylistPath;
            set => this.RaiseAndSetIfChanged(ref _selectedPlaylistPath, value);
        }

        private string _searchTerm;
        public string SearchTerm
        {
            get => _searchTerm;
            set => this.RaiseAndSetIfChanged(ref _searchTerm, value);
        }
        
        private int _totalItems;
        public int TotalItems
        {
            get => _totalItems;
            set => this.RaiseAndSetIfChanged(ref _totalItems, value);
        }

        public ReactiveCommand<string, List<string>> LoadExistingPlaylistsCommand { get; protected set; }
        private readonly ObservableAsPropertyHelper<List<string>> _existingPlaylists;
        public List<string> ExistingPlaylists => _existingPlaylists.Value;

        public ReactiveCommand<string, List<PlaylistElement>> LoadExistingPlaylistCommand { get;
            protected set;
        }
        public ReactiveCommand<string, List<PlaylistElement>> SearchInCurrentPlaylistCommand { get;
            protected set;
        }
        private readonly ObservableAsPropertyHelper<List<PlaylistElement>> _existingPlaylist;
        public List<PlaylistElement> ExistingPlaylist => _existingPlaylist.Value;
        private List<PlaylistElement> _originalPlaylist;

        public ReactiveCommand<Config, Task<bool>> GenerateCurrentPlaylistCommand { get; }

        public PlaylistViewModel()
        {
            LoadExistingPlaylistsCommand = ReactiveCommand.Create<string, List<string>>(LoadExistingPlaylistsFromPath);
            this.WhenAnyValue(x => x.Path)
                .Select(x => x?.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .InvokeCommand(LoadExistingPlaylistsCommand);
            LoadExistingPlaylistsCommand.ToProperty(this, x => x.ExistingPlaylists, out _existingPlaylists, new List<string>());

            LoadExistingPlaylistCommand =
                ReactiveCommand.Create<string, List<PlaylistElement>>(LoadExistingPlaylistFromPath);
            this.WhenAnyValue(x => x.SelectedPlaylistPath)
                .Select(x => x?.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .InvokeCommand(LoadExistingPlaylistCommand);
            LoadExistingPlaylistCommand.ToProperty(this, x => x.ExistingPlaylist, out _existingPlaylist,
                new List<PlaylistElement>());

            SearchInCurrentPlaylistCommand = ReactiveCommand.Create<string, List<PlaylistElement>>(SearchInCurrentPlaylist);
            this.WhenAnyValue(x => x.SearchTerm)
                .Throttle(TimeSpan.FromSeconds(1), RxApp.MainThreadScheduler)
                .Select(x => x?.Trim().ToLower())
                .InvokeCommand(SearchInCurrentPlaylistCommand);
            SearchInCurrentPlaylistCommand.ToProperty(this, x => x.ExistingPlaylist, out _existingPlaylist, new List<PlaylistElement>());

            GenerateCurrentPlaylistCommand = ReactiveCommand.Create<Config, Task<bool>>(GenerateCurrentPlaylist);

            LoadExistingPlaylistsCommand.Subscribe(results =>
            {
                if (ExistingPlaylists.Any())
                {
                    var first = ExistingPlaylists.First();
                    SelectedPlaylistPath = first;
                }
            });

            LoadExistingPlaylistCommand.Subscribe(results =>
            {
                if (results != null && results.Any())
                {
                    SearchTerm = string.Empty;
                }
            });

            GenerateCurrentPlaylistCommand.Subscribe(result =>
            {
                if(result.Result)
                    LoadExistingPlaylistsCommand.Execute();
            });
        }

        private List<PlaylistElement> SearchInCurrentPlaylist(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return _originalPlaylist;

            var returnPlaylist = new List<PlaylistElement>();
            returnPlaylist.AddRange(_originalPlaylist.FindAll(x => x.Track.ToLower().Contains(searchTerm)));
            returnPlaylist.AddRange(_originalPlaylist.FindAll(x => x.User.ToLower().Contains(searchTerm)));
            returnPlaylist.AddRange(_originalPlaylist.FindAll(x => x.TrackId.Equals(searchTerm)));
            foreach (var playlistElement in _originalPlaylist)
            {
                returnPlaylist.AddRange(from playlistElementArtist in playlistElement.Artists
                    where playlistElementArtist.ToLower().Contains(searchTerm)
                    select playlistElement);
                returnPlaylist.AddRange(from playlistElementGenre in playlistElement.Genres
                    where playlistElementGenre.ToLower().Contains(searchTerm)
                    select playlistElement);
            }

            return returnPlaylist.Distinct().ToList();
        }

        private List<PlaylistElement> LoadExistingPlaylistFromPath(string path)
        {
            _originalPlaylist = JsonConvert.DeserializeObject<List<PlaylistElement>>(
                File.ReadAllText(path));
            TotalItems = _originalPlaylist.Count;
            return _originalPlaylist;
        }

        private async Task<bool> GenerateCurrentPlaylist(Config config)
        {
            var spotifyAuthentification = new SpotifyAuthentification();
            var authenticate = spotifyAuthentification.RunAuthentication();
            authenticate.Wait();
            if (!authenticate.Result) return false;
            var playlistHandler = new PlaylistHandler(spotifyAuthentification);
            var creationTask = new Task<bool>(playlistHandler.CreateMixDerWoche);
            creationTask.Start();
            return await creationTask;
        }

        private List<string> LoadExistingPlaylistsFromPath(string path)
        {
            if (Directory.Exists(path))
            {
                var info = new DirectoryInfo(path);
                var files =
                    info.GetFiles("*.json", SearchOption.TopDirectoryOnly)
                        .OrderByDescending(x => x.LastWriteTime)
                        .Select(x => x.FullName)
                        .ToList();
                return files;
            }
            return new List<string>();
        }
    }
}