using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using LiveCharts;
using LiveCharts.Wpf;
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

        public ReactiveCommand<string, ChartElements> LoadSelectedPlaylistCommand { get; protected set; }
        private readonly ObservableAsPropertyHelper<ChartElements> _selectedPlaylist;
        public ChartElements SelectedPlaylist => _selectedPlaylist.Value;

        public ReactiveCommand<string, ChartElements> LoadSelectedPlaylist2Command { get; protected set; }
        private readonly ObservableAsPropertyHelper<ChartElements> _selectedPlaylist2;
        public ChartElements SelectedPlaylist2 => _selectedPlaylist2.Value;

        public ReactiveCommand<string, ChartElements> LoadSelectedPlaylist3Command { get; protected set; }
        private readonly ObservableAsPropertyHelper<ChartElements> _selectedPlaylist3;
        public ChartElements SelectedPlaylist3 => _selectedPlaylist3.Value;

        public ReactiveCommand<string, List<KeyValuePair<string, string>>> LoadExistingPlaylistsCommand { get; protected set; }
        private readonly ObservableAsPropertyHelper<List<KeyValuePair<string, string>>> _existingPlaylists;
        public List<KeyValuePair<string, string>> ExistingPlaylists => _existingPlaylists.Value;

        public DetailsGridViewModel()
        {
            LoadExistingPlaylistsCommand = ReactiveCommand.Create<string, List<KeyValuePair<string, string>>>(LoadExistingPlaylistsFromPath);
            this.WhenAnyValue(x => x.Path)
                .Select(x => x?.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .InvokeCommand(LoadExistingPlaylistsCommand);
            LoadExistingPlaylistsCommand.ToProperty(this, x => x.ExistingPlaylists, out _existingPlaylists,
                new List<KeyValuePair<string, string>>());

            LoadSelectedPlaylistCommand =
                ReactiveCommand.Create<string, ChartElements>(LoadSelectedPlaylist);
            this.WhenAnyValue(x => x.SelectedPlaylistPath)
                .Select(x => x?.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .InvokeCommand(LoadSelectedPlaylistCommand);
            LoadSelectedPlaylistCommand.ToProperty(this, x => x.SelectedPlaylist, out _selectedPlaylist,
                new ChartElements());

            LoadSelectedPlaylist2Command =
                ReactiveCommand.Create<string, ChartElements>(LoadSelectedPlaylist2);
            this.WhenAnyValue(x => x.SelectedPlaylistPath)
                .Select(x => x?.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .InvokeCommand(LoadSelectedPlaylist2Command);
            LoadSelectedPlaylist2Command.ToProperty(this, x => x.SelectedPlaylist2, out _selectedPlaylist2,
                new ChartElements());

            LoadSelectedPlaylist3Command =
                ReactiveCommand.Create<string, ChartElements>(LoadSelectedPlaylist3);
            this.WhenAnyValue(x => x.SelectedPlaylistPath)
                .Select(x => x?.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .InvokeCommand(LoadSelectedPlaylist3Command);
            LoadSelectedPlaylist3Command.ToProperty(this, x => x.SelectedPlaylist3, out _selectedPlaylist3,
                new ChartElements());

            LoadExistingPlaylistsCommand.Subscribe(results =>
            {
                if (ExistingPlaylists.Any())
                {
                    var first = ExistingPlaylists.First();
                    SelectedPlaylistPath = first.Key;
                }
            });
        }

        private List<KeyValuePair<string, string>> LoadExistingPlaylistsFromPath(string path)
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
                var result = new List<KeyValuePair<string, string>>();
                foreach (var file in files)
                {
                    result.Add(new KeyValuePair<string, string>(file,
                        file.Substring(file.LastIndexOf("\\", StringComparison.Ordinal) + 1)));
                }
                return result;
            }
            return new List<KeyValuePair<string, string>>();
        }

        private ChartElements LoadSelectedPlaylist(string path)
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

            string LabelPoint(ChartPoint chartPoint) => $"{chartPoint.SeriesView.Title}";

            var total = 0f;
            var series = new SeriesCollection();
            foreach (var chartElement in dic.Values)
            {
                if (chartElement.PercentageValue >= 15 * percentageValue)
                {
                    series.Add(new PieSeries
                    {
                        Title = $"{chartElement.Name}: {chartElement.Occurrences} ({chartElement.PercentageValue:P})",
                        Values = new ChartValues<double> { chartElement.Occurrences },
                        DataLabels = true,
                        LabelPoint = LabelPoint,
                    });
                    total += chartElement.PercentageValue;
                }
            }
            series.Add(new PieSeries
            {
                Title = $"Others: x ({(float)series.Count / playlist.Count:P})",
                Values = new ChartValues<double> { playlist.Count - series.Count },
                DataLabels = true,
                LabelPoint = LabelPoint
            });

            var chartElements = new ChartElements { SeriesCollection = series, Elements = dic.Values.ToList() };
            return chartElements;
        }

        private ChartElements LoadSelectedPlaylist2(string path)
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

            string LabelPoint(ChartPoint chartPoint) => $"{chartPoint.SeriesView.Title}";

            var total = 0;
            var series = new SeriesCollection();
            foreach (var chartElement in dic.Values)
            {
                if (chartElement.PercentageValue >= 3 * percentageValue)
                {
                    series.Add(new PieSeries
                    {
                        Title = $"{chartElement.Name}: {chartElement.Occurrences} ({chartElement.PercentageValue:P})",
                        Values = new ChartValues<double> { chartElement.Occurrences },
                        DataLabels = true,
                        LabelPoint = LabelPoint,
                    });
                    total += chartElement.Occurrences;
                }
            }
            series.Add(new PieSeries
            {
                Title = $"Others: {playlist.Count - total} ({(double)(playlist.Count - total) / dic.Count:P})",
                Values = new ChartValues<double> { playlist.Count - total },
                DataLabels = true,
                LabelPoint = LabelPoint
            });

            var chartElements = new ChartElements { SeriesCollection = series, Elements = dic.Values.ToList() };
            return chartElements;
        }

        private ChartElements LoadSelectedPlaylist3(string path)
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

            string LabelPoint(ChartPoint chartPoint) => $"{chartPoint.SeriesView.Title}";

            var total = 0;
            var series = new SeriesCollection();
            foreach (var chartElement in dic.Values)
            {
                if (chartElement.PercentageValue >= 3 * percentageValue)
                {
                    series.Add(new PieSeries
                    {
                        Title = $"{chartElement.Name}: {chartElement.Occurrences} ({chartElement.PercentageValue:P})",
                        Values = new ChartValues<double> { chartElement.Occurrences },
                        DataLabels = true,
                        LabelPoint = LabelPoint,
                    });
                    total += chartElement.Occurrences;
                }
            }
            series.Add(new PieSeries
            {
                Title = $"Others: {playlist.Count - total} ({(double)(playlist.Count - total) / dic.Count:P})",
                Values = new ChartValues<double> { playlist.Count - total },
                DataLabels = true,
                LabelPoint = LabelPoint
            });

            var chartElements = new ChartElements { SeriesCollection = series, Elements = dic.Values.ToList() };
            return chartElements;
        }

        private List<PlaylistElement> LoadAllPlaylists()
        {
            var playlist = new List<PlaylistElement>();
            foreach (var existingPlaylist in ExistingPlaylists)
            {
                if(!existingPlaylist.Equals(All))
                    playlist.AddRange(JsonConvert.DeserializeObject<List<PlaylistElement>>(
                        File.ReadAllText(existingPlaylist.Key)));
            }
            return playlist;
        }
    }
}