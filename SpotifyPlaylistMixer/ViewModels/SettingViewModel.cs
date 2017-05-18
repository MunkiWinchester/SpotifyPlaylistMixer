using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using Newtonsoft.Json;
using ReactiveUI;
using SpotifyPlaylistMixer.Business;
using SpotifyPlaylistMixer.DataObjects;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;
using ToastNotifications.Messages;
using ReactiveCommand = ReactiveUI.ReactiveCommand;

namespace SpotifyPlaylistMixer.ViewModels
{
    public class SettingViewModel : ReactiveObject
    {
        private string _path;
        private string _oldPath;
        public string Path
        {
            get => _path;
            set
            {
                _oldPath = _path;
                this.RaiseAndSetIfChanged(ref _path, value);
            }
        }

        private string _pathNewConfig;
        public string PathNewConfig
        {
            get => _pathNewConfig;
            set => this.RaiseAndSetIfChanged(ref _pathNewConfig, value);
        }

        private string _selectedConfigPath;
        public string SelectedConfigPath
        {
            get => _selectedConfigPath;
            set => this.RaiseAndSetIfChanged(ref _selectedConfigPath, value);
        }

        public ReactiveCommand<string, Config> LoadConfigCommand { get; protected set; }
        private readonly ObservableAsPropertyHelper<Config> _config;

        public Config Config => _config.Value;

        public ReactiveCommand<string, List<KeyValuePair<string, string>>> LoadExistingConfigs { get; protected set; }
        private readonly ObservableAsPropertyHelper<List<KeyValuePair<string, string>>> _existingConfigs;
        public List<KeyValuePair<string, string>> ExistingConfigs => _existingConfigs.Value;

        public ReactiveCommand ConfirmCommand { get; }
        public ReactiveCommand<string, string> AddConfigCommand { get; }
        public ReactiveCommand AddUserCommand { get; }
        public ReactiveCommand AddPlaylistToSourceCommand { get; }

        public SettingViewModel()
        {
            LoadExistingConfigs = ReactiveCommand.Create<string, List<KeyValuePair<string, string>>>(LoadExistingConfigsFromPath);
            this.WhenAnyValue(x => x.Path)
                .Throttle(TimeSpan.FromSeconds(1), RxApp.MainThreadScheduler)
                .Select(x => x?.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .InvokeCommand(LoadExistingConfigs);
            _existingConfigs = LoadExistingConfigs.ToProperty(this, x => x.ExistingConfigs, new List<KeyValuePair<string, string>>());

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
            Config.Users.Add(FileHandler.SaveConfigAddUser(LoadExistingConfigsFromPath(Path)));
        }

        private string AddConfig(string dummy)
        {
            var path = Path.EndsWith("\\") ? Path : Path + "\\";
            var file = PathNewConfig.EndsWith(".json") ? path + PathNewConfig : path + PathNewConfig + ".json";
            using (File.Create(file))
            { }
            var json = JsonConvert.SerializeObject(new Config(), Formatting.Indented);
            File.WriteAllText(file, json);
            return file;
        }

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
                {
                    result.Add(new KeyValuePair<string, string>(file,
                        file.Substring(file.LastIndexOf("\\", StringComparison.Ordinal) + 1)));
                }
                return result;
            }
            return new List<KeyValuePair<string, string>>();
        }

        public void ChangeConfig()
        {
            FileHandler.SaveConfigEditUser(LoadExistingConfigsFromPath(Path), Config.Users);
            //TODO SaveCondigEdit für alle Optionen und Notifications aber müssen uns ein anders tool suchen verstehe das aktuelle nicht ganz
            var json = JsonConvert.SerializeObject(Config, Formatting.Indented);
            File.WriteAllText(SelectedConfigPath, json);
        }

        private Config LoadConfigFromPath(string path)
        {
            if (FileHandler.LoadConfig(path) == null)
            {
                Path = _oldPath;
                notifier.ShowWarning("In diesem Ordner befindet sich keine Config.json");
                return Config;
            }
            return FileHandler.LoadConfig(path);
        }

        /*Todo evtl in eigene Klasse*/
        Notifier notifier = new Notifier(cfg =>
        {
            cfg.PositionProvider = new WindowPositionProvider(
                parentWindow: Application.Current.MainWindow,
                corner: Corner.BottomLeft,
                offsetX: 10,
                offsetY: 10);

            cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                notificationLifetime: TimeSpan.FromSeconds(5),
                maximumNotificationCount: MaximumNotificationCount.FromCount(5));

            cfg.Dispatcher = Application.Current.Dispatcher;
        });
    }
}