using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Business.Business;
using SpotifyAPI.Local;
using SpotifyAPI.Local.Enums;
using SpotifyAPI.Local.Models;
using WpfUtility.Services;
using MessageBox = Xceed.Wpf.Toolkit.MessageBox;

namespace UserInterface.ViewModels
{
    public class CurrentPlaybackViewModel : ObservableObject
    {
        private readonly SpotifyLocalHelper _spotify;

        private TimeSpan _currentLength;
        private Dispatcher _dispatcher;

        private ImageSource _image;

        private bool _isConnected;

        private bool _isPlaying;

        private TimeSpan _lenght;

        private double _progress;

        private Track _track;

        public CurrentPlaybackViewModel()
        {
            _spotify = new SpotifyLocalHelper();
            IsConnected = false;
        }

        public Track Track
        {
            get => _track;
            set
            {
                _track = value;
                OnPropertyChanged();
            }
        }

        public bool IsConnected
        {
            get => _isConnected;
            set
            {
                _isConnected = value;
                OnPropertyChanged();
            }
        }

        public bool IsPlaying
        {
            get => _isPlaying;
            set
            {
                _isPlaying = value;
                OnPropertyChanged();
            }
        }

        public ImageSource Image
        {
            get => _image;
            set
            {
                _image = value;
                OnPropertyChanged();
            }
        }

        public TimeSpan Length
        {
            get => _lenght;
            set
            {
                _lenght = value;
                OnPropertyChanged();
            }
        }

        public TimeSpan CurrentLength
        {
            get => _currentLength;
            set
            {
                _currentLength = value;
                OnPropertyChanged();
            }
        }

        public double Progress
        {
            get => _progress;
            set
            {
                _progress = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand<string> NavigateUriCommand => new RelayCommand<string>(NavigateUri);

        public void InitializeViewModel()
        {
            _spotify.Connect();
            _spotify.OnTrackChange += SpotifyOnOnTrackChange;
            _spotify.OnTrackTimeChange += SpotifyOnOnTrackTimeChange;
            _spotify.OnPlayStateChange += SpotifyOnOnPlayStateChange;
        }

        private void SpotifyOnOnTrackTimeChange(object sender, TrackTimeChangeEventArgs e)
        {
            var timeSpan = TimeSpan.FromSeconds(e.TrackTime);
            CurrentLength = new TimeSpan(timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
            Progress = CurrentLength.TotalSeconds / Length.TotalSeconds * 100;
        }

        private void SpotifyOnOnTrackChange(object sender, TrackChangeEventArgs e)
        {
            SetTrack(e.NewTrack);
        }

        private void SpotifyOnOnPlayStateChange(object sender, PlayStateEventArgs e)
        {
            IsPlaying = e.Playing;
        }

        public void NavigateUri(string uri)
        {
            if (!string.IsNullOrWhiteSpace(uri))
                Process.Start(uri);
        }

        public void LoadValues(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
            if (!_spotify.IsConnected)
            {
                IsConnected = false;
                var result = MessageBox.Show(@"Couldn't connect to the spotify client. Retry?", @"Spotify",
                    MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    _spotify.Connect();
                    LoadValues(dispatcher);
                }
            }
            else
            {
                IsConnected = true;
                SetTrack(_spotify.UpdateInfos());
            }
        }

        private async void SetTrack(Track track)
        {
            Track = track;
            Length = TimeSpan.FromSeconds(track.Length);
            try
            {
                var image = ToImageSource(await Track.GetAlbumArtAsByteArrayAsync(AlbumArtSize.Size320));
                await _dispatcher.BeginInvoke(new Action(() => Image = image));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static BitmapImage ToImageSource(byte[] array)
        {
            using (var ms = new MemoryStream(array))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = ms;
                image.EndInit();
                image.Freeze();
                return image;
            }
        }
    }
}