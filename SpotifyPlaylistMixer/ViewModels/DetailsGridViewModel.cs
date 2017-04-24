using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Documents;
using Newtonsoft.Json;
using ReactiveUI;
using SpotifyPlaylistMixer.Converter;
using SpotifyPlaylistMixer.DataObjects;

namespace SpotifyPlaylistMixer.ViewModels
{
    public class DetailsGridViewModel : ReactiveObject
    {
        private const string All = "- ALL -";

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

        private int _totalItems;

        public int TotalItems
        {
            get => _totalItems;
            set => this.RaiseAndSetIfChanged(ref _totalItems, value);
        }

        private float _percentagePerEntry;

        public float PercentagePerEntry
        {
            get => _percentagePerEntry;
            set => this.RaiseAndSetIfChanged(ref _percentagePerEntry, value);
        }

        public ReactiveCommand<string, List<ChartElement>> LoadSelectedPlaylistCommand { get; protected set; }
        private readonly ObservableAsPropertyHelper<List<ChartElement>> _selectedPlaylist;
        public List<ChartElement> SelectedPlaylist => _selectedPlaylist.Value;

        public ReactiveCommand<string, List<ChartElement>> LoadSelectedPlaylist2Command { get; protected set; }
        private readonly ObservableAsPropertyHelper<List<ChartElement>> _selectedPlaylist2;
        public List<ChartElement> SelectedPlaylist2 => _selectedPlaylist2.Value;

        public ReactiveCommand<string, List<ChartElement>> LoadSelectedPlaylist3Command { get; protected set; }
        private readonly ObservableAsPropertyHelper<List<ChartElement>> _selectedPlaylist3;
        public List<ChartElement> SelectedPlaylist3 => _selectedPlaylist3.Value;

        public ReactiveCommand<string, List<string>> LoadExistingPlaylistsCommand { get; protected set; }
        private readonly ObservableAsPropertyHelper<List<string>> _existingPlaylists;
        public List<string> ExistingPlaylists => _existingPlaylists.Value;

        public DetailsGridViewModel()
        {
            LoadExistingPlaylistsCommand = ReactiveCommand.Create<string, List<string>>(LoadExistingPlaylistsFromPath);
            this.WhenAnyValue(x => x.Path)
                .Select(x => x?.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .InvokeCommand(LoadExistingPlaylistsCommand);
            LoadExistingPlaylistsCommand.ToProperty(this, x => x.ExistingPlaylists, out _existingPlaylists,
                new List<string>());

            LoadSelectedPlaylistCommand =
                ReactiveCommand.Create<string, List<ChartElement>>(LoadSelectedPlaylist);
            this.WhenAnyValue(x => x.SelectedPlaylistPath)
                .Select(x => x?.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .InvokeCommand(LoadSelectedPlaylistCommand);
            LoadSelectedPlaylistCommand.ToProperty(this, x => x.SelectedPlaylist, out _selectedPlaylist,
                new List<ChartElement>());

            LoadSelectedPlaylist2Command =
                ReactiveCommand.Create<string, List<ChartElement>>(LoadSelectedPlaylist2);
            this.WhenAnyValue(x => x.SelectedPlaylistPath)
                .Select(x => x?.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .InvokeCommand(LoadSelectedPlaylist2Command);
            LoadSelectedPlaylist2Command.ToProperty(this, x => x.SelectedPlaylist2, out _selectedPlaylist2,
                new List<ChartElement>());

            LoadSelectedPlaylist3Command =
                ReactiveCommand.Create<string, List<ChartElement>>(LoadSelectedPlaylist3);
            this.WhenAnyValue(x => x.SelectedPlaylistPath)
                .Select(x => x?.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .InvokeCommand(LoadSelectedPlaylist3Command);
            LoadSelectedPlaylist3Command.ToProperty(this, x => x.SelectedPlaylist3, out _selectedPlaylist3,
                new List<ChartElement>());

            LoadExistingPlaylistsCommand.Subscribe(results =>
            {
                if (ExistingPlaylists.Any())
                {
                    var first = ExistingPlaylists.First();
                    SelectedPlaylistPath = first;
                }
            });
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
                files.Add(All);
                return files;
            }
            return new List<string>();
        }

        private List<ChartElement> LoadSelectedPlaylist(string path)
        {
            List<PlaylistElement> playlist;
            if (path.Equals(All))
                playlist = LoadAllPlaylists();
            else
                playlist = JsonConvert.DeserializeObject<List<PlaylistElement>>(
                    File.ReadAllText(path));
            var percentageValue = (float) 1 / playlist.Count;
            PercentagePerEntry = percentageValue;
            TotalItems = playlist.Count;
            var dic = new ConcurrentDictionary<string, ChartElement>();
            foreach (var playlistElement in playlist)
            {
                foreach (var genre in playlistElement.Genres)
                {
                    dic.AddOrUpdate(genre,
                        new ChartElement {Name = genre, PercentageValue = percentageValue, Occurrences = 1},
                        (key, oldValue) =>
                            new ChartElement
                            {
                                Name = oldValue.Name,
                                PercentageValue = oldValue.PercentageValue + percentageValue,
                                Occurrences = oldValue.Occurrences + 1
                            }
                    );
                }
            }
            return dic.Values.ToList();
        }

        private List<ChartElement> LoadSelectedPlaylist2(string path)
        {
            List<PlaylistElement> playlist;
            if (path.Equals(All))
                playlist = LoadAllPlaylists();
            else
                playlist = JsonConvert.DeserializeObject<List<PlaylistElement>>(
                    File.ReadAllText(path));
            var percentageValue = (float) 1 / playlist.Count;
            PercentagePerEntry = percentageValue;
            TotalItems = playlist.Count;
            var dic = new ConcurrentDictionary<string, ChartElement>();
            foreach (var playlistElement in playlist)
            {
                foreach (var artist in playlistElement.Artists)
                {
                    dic.AddOrUpdate(artist,
                        new ChartElement {Name = artist, PercentageValue = percentageValue, Occurrences = 1},
                        (key, oldValue) =>
                            new ChartElement
                            {
                                Name = oldValue.Name,
                                PercentageValue = oldValue.PercentageValue + percentageValue,
                                Occurrences = oldValue.Occurrences + 1
                            }
                    );
                }
            }
            return dic.Values.ToList();
        }

        private List<ChartElement> LoadSelectedPlaylist3(string path)
        {
            List<PlaylistElement> playlist;
            if (path.Equals(All))
                playlist = LoadAllPlaylists();
            else
                playlist = JsonConvert.DeserializeObject<List<PlaylistElement>>(
                    File.ReadAllText(path));
            var percentageValue = (float) 1 / playlist.Count;
            PercentagePerEntry = percentageValue;
            TotalItems = playlist.Count;
            var dic = new ConcurrentDictionary<string, ChartElement>();
            foreach (var playlistElement in playlist)
            {
                var name = $"{playlistElement.Track}\r\n({ListStringToStringConverter.ConnectedString(playlistElement.Artists)})";
                dic.AddOrUpdate(name,
                    new ChartElement {Name = name, PercentageValue = percentageValue, Occurrences = 1},
                    (key, oldValue) =>
                        new ChartElement
                        {
                            Name = oldValue.Name,
                            PercentageValue = oldValue.PercentageValue + percentageValue,
                            Occurrences = oldValue.Occurrences + 1
                        }
                );
            }
            return dic.Values.ToList();
        }

        private List<PlaylistElement> LoadAllPlaylists()
        {
            var playlist = new List<PlaylistElement>();
            foreach (var existingPlaylist in ExistingPlaylists)
            {
                if(!existingPlaylist.Equals(All))
                    playlist.AddRange(JsonConvert.DeserializeObject<List<PlaylistElement>>(
                        File.ReadAllText(existingPlaylist)));
            }
            return playlist;
        }
    }
}