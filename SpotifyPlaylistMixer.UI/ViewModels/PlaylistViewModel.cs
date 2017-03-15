using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using ReactiveUI;
using SpotifyPlaylistMixer.Business;
using SpotifyPlaylistMixer.DataObjects;

namespace SpotifyPlaylistMixer.UI.ViewModels
{
    public class PlaylistViewModel : ReactiveObject
    {
        private string _path;

        public string Path
        {
            get { return _path; }
            set { this.RaiseAndSetIfChanged(ref _path, value); }
        }

        private string _selectedPlaylist;

        public string SelectedPlaylist
        {
            get { return _selectedPlaylist; }
            set { this.RaiseAndSetIfChanged(ref _selectedPlaylist, value); }
        }

        public ReactiveCommand<string, List<string>> LoadExistingPlaylists { get; protected set; }
        private readonly ObservableAsPropertyHelper<List<string>> _existingPlaylists;
        public List<string> ExistingPlaylists => _existingPlaylists.Value;

        public ReactiveCommand<string, List<PlaylistElement>> LoadExistingPlaylist { get;
            protected set; }
        private readonly ObservableAsPropertyHelper<List<PlaylistElement>> _existingPlaylist;
        public List<PlaylistElement> ExistingPlaylist => _existingPlaylist.Value;

        public ReactiveCommand GenerateCurrentPlaylistCommand { get; private set; }

        public PlaylistViewModel()
        {
            LoadExistingPlaylists = ReactiveCommand.Create<string, List<string>>(LoadExistingPlaylistsFromPath);
            this.WhenAnyValue(x => x.Path)
                .Select(x => x?.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .InvokeCommand(LoadExistingPlaylists);
            _existingPlaylists = LoadExistingPlaylists.ToProperty(this, x => x.ExistingPlaylists, new List<string>());

            LoadExistingPlaylist =
                ReactiveCommand.Create<string, List<PlaylistElement>>(LoadExistingPlaylistFromPath);
            this.WhenAnyValue(x => x.SelectedPlaylist)
                .Select(x => x?.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .InvokeCommand(LoadExistingPlaylist);
            _existingPlaylist = LoadExistingPlaylist.ToProperty(this, x => x.ExistingPlaylist,
                new List<PlaylistElement>());

            GenerateCurrentPlaylistCommand = ReactiveCommand.Create(GenerateCurrentPlaylist);
        }

        private List<PlaylistElement> LoadExistingPlaylistFromPath(string path)
        {
            var elements = JsonConvert.DeserializeObject<IEnumerable<KeyValuePair<string, List<string>>>>(
                File.ReadAllText(path)).ToList();
            var list = new List<PlaylistElement>();
            foreach (var element in elements)
            {
                // TODO: mock this to real "playlist" files and implement the json properly
                list.AddRange(
                    element.Value.Select(song => Regex.Split(song, " --- "))
                        .Select(
                            spli =>
                                new PlaylistElement
                                {
                                    User = element.Key,
                                    Artists = new List<string> { spli[0] },
                                    Track = spli[1],
                                    Genres = new List<string> { "death" }
                                    }));
            }
            return list;
        }

        private void GenerateCurrentPlaylist()
        {
            var spotifyAuthentification = new SpotifyAuthentification();
            var authenticate = spotifyAuthentification.RunAuthentication();
            authenticate.Wait();
            if (authenticate.Result)
            {
                var playlistHandler = new PlaylistHandler(spotifyAuthentification);
                playlistHandler.CreateMixDerWoche();
            }
        }

        private List<string> LoadExistingPlaylistsFromPath(string path)
        {
            if (Directory.Exists(path))
            {
                var info = new DirectoryInfo(path);
                var files =
                    info.GetFiles("*.json", SearchOption.TopDirectoryOnly)
                        .OrderByDescending(x => x.CreationTime)
                        .Select(x => x.FullName)
                        .ToList();
                return files;
            }
            return new List<string>();
        }
    }
}