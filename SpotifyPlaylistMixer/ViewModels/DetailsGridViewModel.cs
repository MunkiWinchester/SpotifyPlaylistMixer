using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using LiveCharts;
using LiveCharts.Wpf;
using Newtonsoft.Json;
using SpotifyPlaylistMixer.Business;
using SpotifyPlaylistMixer.DataObjects;
using WpfUtility.Services;

namespace SpotifyPlaylistMixer.ViewModels
{
    public class DetailsGridViewModel : ObservableObject
    {
        private const string All = "- ALL -";
        private readonly KeyValuePair<string, string> _allKvP = new KeyValuePair<string, string>(All, All);
        private ObservableCollection<KeyValuePair<string, string>> _existingPlaylists;
        private float _percentagePerEntry;
        private string _searchTerm;

        private ChartElements _selectedPlaylist;
        private ChartElements _selectedPlaylist2;
        private ChartElements _selectedPlaylist3;
        private string _selectedPlaylistPath;
        private int _totalItems;

        public float PercentagePerEntry
        {
            get => _percentagePerEntry;
            set
            {
                _percentagePerEntry = value;
                OnPropertyChanged();
            }
        }

        public string SelectedPlaylistPath
        {
            get => _selectedPlaylistPath;
            set
            {
                _selectedPlaylistPath = value;
                LoadSelectedPlaylist(value);
                LoadSelectedPlaylist2(value);
                LoadSelectedPlaylist3(value);
                OnPropertyChanged();
            }
        }

        public string SearchTerm
        {
            get => _searchTerm;
            set
            {
                _searchTerm = value;
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

        public ChartElements SelectedPlaylist
        {
            get => _selectedPlaylist;
            set
            {
                _selectedPlaylist = value;
                OnPropertyChanged();
            }
        }

        public ChartElements SelectedPlaylist2
        {
            get => _selectedPlaylist2;
            set
            {
                _selectedPlaylist2 = value;
                OnPropertyChanged();
            }
        }

        public ChartElements SelectedPlaylist3
        {
            get => _selectedPlaylist3;
            set
            {
                _selectedPlaylist3 = value;
                OnPropertyChanged();
            }
        }

        public void LoadExistingPlaylistsFromPath(string path)
        {
            var playlists =
                new ObservableCollection<KeyValuePair<string, string>>(FileHandler
                    .LoadExistingPlaylistsFromPath(path)) {_allKvP};
            ExistingPlaylists = playlists;
            var first = ExistingPlaylists.First();
            SelectedPlaylistPath = first.Key;
        }

        private void LoadSelectedPlaylist(string path)
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
            foreach (var genre in playlistElement.Genres)
                dic.AddOrUpdate(genre,
                    new ChartElement
                    {
                        Name = genre,
                        PercentageValue = percentageValue,
                        Occurrences = 1,
                        OccurrenceIn = new List<PlaylistElement> {playlistElement}
                    },
                    (key, oldValue) =>
                    {
                        var x = new List<PlaylistElement>();
                        x.AddRange(oldValue.OccurrenceIn);
                        x.Add(playlistElement);
                        return new ChartElement
                        {
                            Name = oldValue.Name,
                            PercentageValue = oldValue.PercentageValue + percentageValue,
                            Occurrences = oldValue.Occurrences + 1,
                            OccurrenceIn = x
                        };
                    }
                );

            string LabelPoint(ChartPoint chartPoint)
            {
                return $"{chartPoint.SeriesView.Title}";
            }

            var series = new SeriesCollection();
            foreach (var chartElement in dic.Values)
                if (chartElement.PercentageValue >= 15 * percentageValue)
                    series.Add(new PieSeries
                    {
                        Title = $"{chartElement.Name}: {chartElement.Occurrences} ({chartElement.PercentageValue:P})",
                        Values = new ChartValues<double> {chartElement.Occurrences},
                        DataLabels = true,
                        LabelPoint = LabelPoint
                    });
            series.Add(new PieSeries
            {
                Title = $"Others: x ({(float) series.Count / playlist.Count:P})",
                Values = new ChartValues<double> {playlist.Count - series.Count},
                DataLabels = true,
                LabelPoint = LabelPoint
            });
            SelectedPlaylist = new ChartElements {SeriesCollection = series, Elements = dic.Values.ToList()};
        }

        private void LoadSelectedPlaylist2(string path)
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
            foreach (var artist in playlistElement.Artists)
                dic.AddOrUpdate(artist,
                    new ChartElement
                    {
                        Name = artist,
                        PercentageValue = percentageValue,
                        Occurrences = 1,
                        OccurrenceIn = new List<PlaylistElement> {playlistElement}
                    },
                    (key, oldValue) =>
                    {
                        var x = new List<PlaylistElement>();
                        x.AddRange(oldValue.OccurrenceIn);
                        x.Add(playlistElement);
                        return new ChartElement
                        {
                            Name = oldValue.Name,
                            PercentageValue = oldValue.PercentageValue + percentageValue,
                            Occurrences = oldValue.Occurrences + 1,
                            OccurrenceIn = x
                        };
                    }
                );

            string LabelPoint(ChartPoint chartPoint)
            {
                return $"{chartPoint.SeriesView.Title}";
            }

            var total = 0;
            var series = new SeriesCollection();
            foreach (var chartElement in dic.Values)
                if (chartElement.PercentageValue >= 3 * percentageValue)
                {
                    series.Add(new PieSeries
                    {
                        Title = $"{chartElement.Name}: {chartElement.Occurrences} ({chartElement.PercentageValue:P})",
                        Values = new ChartValues<double> {chartElement.Occurrences},
                        DataLabels = true,
                        LabelPoint = LabelPoint
                    });
                    total += chartElement.Occurrences;
                }
            series.Add(new PieSeries
            {
                Title = $"Others: {playlist.Count - total} ({(double) (playlist.Count - total) / dic.Count:P})",
                Values = new ChartValues<double> {playlist.Count - total},
                DataLabels = true,
                LabelPoint = LabelPoint
            });
            SelectedPlaylist2 = new ChartElements {SeriesCollection = series, Elements = dic.Values.ToList()};
        }

        private void LoadSelectedPlaylist3(string path)
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
                var name = $"{playlistElement.Track}\r\n({playlistElement.Artists.ToConnectedString()})";
                dic.AddOrUpdate(name,
                    new ChartElement
                    {
                        Name = name,
                        PercentageValue = percentageValue,
                        Occurrences = 1,
                        OccurrenceIn = new List<PlaylistElement> {playlistElement}
                    },
                    (key, oldValue) =>
                    {
                        var x = new List<PlaylistElement>();
                        x.AddRange(oldValue.OccurrenceIn);
                        x.Add(playlistElement);
                        return
                            new ChartElement
                            {
                                Name = oldValue.Name,
                                PercentageValue = oldValue.PercentageValue + percentageValue,
                                Occurrences = oldValue.Occurrences + 1,
                                OccurrenceIn = x
                            };
                    }
                );
            }

            string LabelPoint(ChartPoint chartPoint)
            {
                return $"{chartPoint.SeriesView.Title}";
            }

            var total = 0;
            var series = new SeriesCollection();
            foreach (var chartElement in dic.Values)
                if (chartElement.PercentageValue >= 3 * percentageValue)
                {
                    series.Add(new PieSeries
                    {
                        Title = $"{chartElement.Name}: {chartElement.Occurrences} ({chartElement.PercentageValue:P})",
                        Values = new ChartValues<double> {chartElement.Occurrences},
                        DataLabels = true,
                        LabelPoint = LabelPoint
                    });
                    total += chartElement.Occurrences;
                }
            series.Add(new PieSeries
            {
                Title = $"Others: {playlist.Count - total} ({(double) (playlist.Count - total) / dic.Count:P})",
                Values = new ChartValues<double> {playlist.Count - total},
                DataLabels = true,
                LabelPoint = LabelPoint
            });
            SelectedPlaylist3 = new ChartElements {SeriesCollection = series, Elements = dic.Values.ToList()};
        }

        private List<PlaylistElement> LoadAllPlaylists()
        {
            var playlist = new List<PlaylistElement>();
            foreach (var existingPlaylist in ExistingPlaylists)
                if (!existingPlaylist.Key.Equals(_allKvP.Key))
                    playlist.AddRange(JsonConvert.DeserializeObject<List<PlaylistElement>>(
                        File.ReadAllText(existingPlaylist.Key)));
            return playlist;
        }
    }
}