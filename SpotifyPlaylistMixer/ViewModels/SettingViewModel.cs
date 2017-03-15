using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using Newtonsoft.Json;
using ReactiveUI;
using SpotifyPlaylistMixer.Business;
using SpotifyPlaylistMixer.DataObjects;
using ReactiveCommand = ReactiveUI.ReactiveCommand;

namespace SpotifyPlaylistMixer.ViewModels
{
    public class SettingViewModel : ReactiveObject
    {
        private string _path;
        public string Path
        {
            get { return _path; }
            set { this.RaiseAndSetIfChanged(ref _path, value); }
        }

        private string _pathNewConfig;
        public string PathNewConfig
        {
            get { return _pathNewConfig; }
            set { this.RaiseAndSetIfChanged(ref _pathNewConfig, value); }
        }

        private string _selectedConfigPath;
        public string SelectedConfigPath
        {
            get { return _selectedConfigPath; }
            set { this.RaiseAndSetIfChanged(ref _selectedConfigPath, value); }
        }

        public ReactiveCommand<string, Config> LoadConfigCommand { get; protected set; }
        private readonly ObservableAsPropertyHelper<Config> _config;
        public Config Config => _config.Value;

        public ReactiveCommand<string, List<string>> LoadExistingConfigs { get; protected set; }
        private readonly ObservableAsPropertyHelper<List<string>> _existingConfigs;
        public List<string> ExistingConfigs => _existingConfigs.Value;

        public ReactiveCommand ConfirmCommand { get; private set; }
        public ReactiveCommand<string, string> AddConfigCommand { get; private set; }
        public ReactiveCommand AddUserCommand { get; private set; }
        public ReactiveCommand AddPlaylistToSourceCommand { get; private set; }

        public SettingViewModel()
        {
            LoadExistingConfigs = ReactiveCommand.Create<string, List<string>>(LoadExistingConfigsFromPath);
            this.WhenAnyValue(x => x.Path)
                .Throttle(TimeSpan.FromSeconds(1), RxApp.MainThreadScheduler)
                .Select(x => x?.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .InvokeCommand(LoadExistingConfigs);
            _existingConfigs = LoadExistingConfigs.ToProperty(this, x => x.ExistingConfigs, new List<string>());

            LoadExistingConfigs.Subscribe(results =>
            {
                if (ExistingConfigs.Any())
                {
                    var first = ExistingConfigs.First();
                    SelectedConfigPath = first;
                }
            });

            LoadConfigCommand = ReactiveCommand.Create<string, Config>(LoadConfigFromPath);
            this.WhenAnyValue(x => x.SelectedConfigPath)
                .Select(x => x?.Trim())
                .DistinctUntilChanged(x => x)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .InvokeCommand(LoadConfigCommand);
            _config = LoadConfigCommand.ToProperty(this, x => x.Config, new Config());

            // TODO: Regelwerk anpassen
            var canConfirmConfigObservable = this.WhenAny(vm => vm.Config,
                s =>
                    true
                    //!string.IsNullOrEmpty(s.Value.TargetPlaylist?.Identifier)
                    //&& !string.IsNullOrEmpty(s.Value.TargetPlaylist.Owner.Identifier)
                    //&& s.Value.SourcePlaylists.Any()
                    //&& !string.IsNullOrEmpty(s.Value.SourcePlaylists.FirstOrDefault()?.Name)
                    //&& !string.IsNullOrEmpty(s.Value.SourcePlaylists.FirstOrDefault()?.Owner.Identifier)
                );
            ConfirmCommand = ReactiveCommand.Create(ChangeConfig, canConfirmConfigObservable);

            var canAddConfigObservable = this.WhenAny(vm => vm.PathNewConfig,
                pnc => !string.IsNullOrEmpty(pnc.Value) && !File.Exists(pnc.Value));
            AddConfigCommand = ReactiveCommand.Create<string, string>(AddConfig, canAddConfigObservable);

            AddConfigCommand.Subscribe(result =>
            {
                if (!string.IsNullOrEmpty(result))
                {
                    LoadExistingConfigs.Execute();
                    SelectedConfigPath = result;
                }
            });

            AddUserCommand = ReactiveCommand.Create(AddUser);

            AddPlaylistToSourceCommand = ReactiveCommand.Create(AddPlaylistToSource);
        }

        private void AddPlaylistToSource()
        {
            Config.SourcePlaylists.Add(new Playlist());
        }

        private void AddUser()
        {
            Config.Users.Add(new User());
        }

        private string AddConfig(string dummy)
        {
            var path = Path.EndsWith("\\") ? Path : Path + "\\";
            var file = PathNewConfig.EndsWith(".json") ? path + PathNewConfig : path + PathNewConfig + ".json";
            using (var stream = File.Create(file)) { }
            var json = JsonConvert.SerializeObject(new Config(), Formatting.Indented);
            File.WriteAllText(file, json);
            return file;
        }

        private List<string> LoadExistingConfigsFromPath(string path)
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

        public void ChangeConfig()
        {
            var ja = "here";
        }

        private Config LoadConfigFromPath(string path)
        {
            return FileHandler.LoadConfig(path);
        }
    }
}