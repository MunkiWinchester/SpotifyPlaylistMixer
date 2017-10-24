using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Business.Business;
using DataObjects.DataObjects;
using WpfUtility.Services;

namespace UserInterface.ViewModels
{
    public class GeneratePlaylistViewModel : ObservableObject
    {
        private Config _config;
        private ObservableCollection<KeyValuePair<string, string>> _existingConfigs;
        private bool _isNotBusy;

        private string _path;

        private string _selectedConfigPath;

        public GeneratePlaylistViewModel()
        {
            var dir = Directory.GetCurrentDirectory();
            Path = $@"{dir}\Resources\Examples\Config\";
            LoadExistingConfigsFromPath();
            IsNotBusy = true;
        }

        public bool IsNotBusy
        {
            get => _isNotBusy;
            set
            {
                _isNotBusy = value;
                OnPropertyChanged();
            }
        }

        public string Path
        {
            get => _path;
            set
            {
                _path = value;
                OnPropertyChanged();
            }
        }

        public string SelectedConfigPath
        {
            get => _selectedConfigPath;
            set
            {
                _selectedConfigPath = value;
                LoadConfigFromPath();
                OnPropertyChanged();
            }
        }

        public ICommand LoadConfigCommand => new DelegateCommand(LoadConfigFromPath);

        public Config Config
        {
            get => _config;
            set
            {
                _config = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoadExistingConfigs => new DelegateCommand(LoadExistingConfigsFromPath);

        public ObservableCollection<KeyValuePair<string, string>> ExistingConfigs
        {
            get => _existingConfigs;
            set => SetField(ref _existingConfigs, value);
        }

        public ICommand GenerateCurrentPlaylistCommand =>
            new DelegateCommand(async () => await GenerateCurrentPlaylist()
                .ContinueWith(t =>
                {
                    if (t.IsCompleted) IsNotBusy = true;
                }, TaskContinuationOptions.AttachedToParent));

        private void LoadExistingConfigsFromPath()
        {
            if (Directory.Exists(Path))
            {
                var info = new DirectoryInfo(Path);
                var files =
                    info.GetFiles("*.json", SearchOption.TopDirectoryOnly)
                        .OrderByDescending(x => x.CreationTime)
                        .Select(x => x.FullName)
                        .ToList();
                var result = new List<KeyValuePair<string, string>>();
                foreach (var file in files)
                    result.Add(new KeyValuePair<string, string>(file,
                        file.Substring(file.LastIndexOf("\\", StringComparison.Ordinal) + 1)));
                ExistingConfigs = new ObservableCollection<KeyValuePair<string, string>>(result);
                SelectedConfigPath = ExistingConfigs.FirstOrDefault().Key;
                LoadConfigFromPath();
            }
            else
            {
                ExistingConfigs =
                    new ObservableCollection<KeyValuePair<string, string>>(new List<KeyValuePair<string, string>>());
            }
        }

        private void LoadConfigFromPath()
        {
            Config = FileHandler.LoadConfig(SelectedConfigPath);
        }

        private async Task GenerateCurrentPlaylist()
        {
            IsNotBusy = false;
            var spotifyAuthentification = new SpotifyAuthentification();
            var authenticate = spotifyAuthentification.RunAuthentication();
            authenticate.Wait();
            if (!authenticate.Result) return;
            var playlistHandler = new PlaylistHandler(spotifyAuthentification);
            var creationTask = new Task(() => playlistHandler.CreateMixDerWoche(Config));
            creationTask.Start();
            await creationTask;
        }
    }
}