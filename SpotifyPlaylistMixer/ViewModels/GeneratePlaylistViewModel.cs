using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ReactiveUI;
using SpotifyPlaylistMixer.Business;
using SpotifyPlaylistMixer.DataObjects;

namespace SpotifyPlaylistMixer.ViewModels
{
    public class GeneratePlaylistViewModel : ReactiveObject
    {
        private readonly ObservableAsPropertyHelper<Config> _config;
        private readonly ObservableAsPropertyHelper<List<KeyValuePair<string, string>>> _existingConfigs;
        private bool _isNotBusy;

        private string _path;

        private string _selectedConfigPath;

        public GeneratePlaylistViewModel()
        {
            IsNotBusy = true;
            LoadExistingConfigs =
                ReactiveCommand.Create<string, List<KeyValuePair<string, string>>>(LoadExistingConfigsFromPath);
            this.WhenAnyValue(x => x.Path)
                .Throttle(TimeSpan.FromSeconds(1), RxApp.MainThreadScheduler)
                .Select(x => x?.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .InvokeCommand(LoadExistingConfigs);
            _existingConfigs = LoadExistingConfigs.ToProperty(this, x => x.ExistingConfigs,
                new List<KeyValuePair<string, string>>());

            LoadExistingConfigs.Subscribe(results =>
            {
                if (ExistingConfigs.Any())
                {
                    var first = ExistingConfigs.First();
                    SelectedConfigPath = first.Key;
                }
            });

            LoadConfigCommand = ReactiveCommand.Create<string, Config>(LoadConfigFromPath);
            this.WhenAnyValue(x => x.SelectedConfigPath)
                .Select(x => x?.Trim())
                .DistinctUntilChanged(x => x)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .InvokeCommand(LoadConfigCommand);
            _config = LoadConfigCommand.ToProperty(this, x => x.Config, new Config());

            GenerateCurrentPlaylistCommand = ReactiveCommand.Create<Config, Task<bool>>(GenerateCurrentPlaylist);
        }

        public bool IsNotBusy
        {
            get => _isNotBusy;
            set => this.RaiseAndSetIfChanged(ref _isNotBusy, value);
        }

        public string Path
        {
            get => _path;
            set => this.RaiseAndSetIfChanged(ref _path, value);
        }

        public string SelectedConfigPath
        {
            get => _selectedConfigPath;
            set => this.RaiseAndSetIfChanged(ref _selectedConfigPath, value);
        }

        public ReactiveCommand<string, Config> LoadConfigCommand { get; protected set; }
        public Config Config => _config.Value;

        public ReactiveCommand<string, List<KeyValuePair<string, string>>> LoadExistingConfigs { get; protected set; }
        public List<KeyValuePair<string, string>> ExistingConfigs => _existingConfigs.Value;

        public ReactiveCommand<Config, Task<bool>> GenerateCurrentPlaylistCommand { get; }

        private List<KeyValuePair<string, string>> LoadExistingConfigsFromPath(string path)
        {
            if (Directory.Exists(path))
            {
                var info = new DirectoryInfo(path);
                var files =
                    info.GetFiles("*.json", SearchOption.TopDirectoryOnly)
                        .OrderByDescending(x => x.CreationTime)
                        .Select(x => x.FullName)
                        .ToList();
                var result = new List<KeyValuePair<string, string>>();
                foreach (var file in files)
                    result.Add(new KeyValuePair<string, string>(file,
                        file.Substring(file.LastIndexOf("\\", StringComparison.Ordinal) + 1)));
                return result;
            }
            return new List<KeyValuePair<string, string>>();
        }

        private Config LoadConfigFromPath(string path)
        {
            return FileHandler.LoadConfig(path);
        }

        private async Task<bool> GenerateCurrentPlaylist(Config config)
        {
            IsNotBusy = false;
            var spotifyAuthentification = new SpotifyAuthentification();
            var authenticate = spotifyAuthentification.RunAuthentication();
            authenticate.Wait();
            if (!authenticate.Result) return false;
            var playlistHandler = new PlaylistHandler(spotifyAuthentification);
            var creationTask = new Task<bool>(() => playlistHandler.CreateMixDerWoche(Config));
            creationTask.Start();
            return await creationTask;
        }
    }
}