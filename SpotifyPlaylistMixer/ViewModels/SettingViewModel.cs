using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Newtonsoft.Json;
using SpotifyPlaylistMixer.Business;
using SpotifyPlaylistMixer.DataObjects;
using WpfUtility.Services;

namespace SpotifyPlaylistMixer.ViewModels
{
    public class SettingViewModel : ObservableObject
    {
        private Config _config;
        private ObservableCollection<KeyValuePair<string, string>> _existingConfigs;

        /*Todo evtl in eigene Klasse*/
        //private readonly Notifier _notifier = new Notifier(cfg =>
        //{
        //    cfg.PositionProvider = new WindowPositionProvider(
        //        Application.Current.MainWindow,
        //        Corner.BottomLeft,
        //        10,
        //        10);

        //    cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
        //        TimeSpan.FromSeconds(5),
        //        MaximumNotificationCount.FromCount(5));

        //    cfg.Dispatcher = Application.Current.Dispatcher;
        //});

        private string _oldPath;
        private string _path;

        private string _pathNewConfig;

        private string _selectedConfigPath;

        public SettingViewModel()
        {
            var dir = Directory.GetCurrentDirectory();
            Path = $@"{dir}\Resources\Examples\Config\";
            LoadExistingConfigsFromPath();
        }

        public string Path
        {
            get => _path;
            set
            {
                _oldPath = _path;
                _path = value;
                OnPropertyChanged();
            }
        }

        public string PathNewConfig
        {
            get => _pathNewConfig;
            set
            {
                _pathNewConfig = value;
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

        public ICommand ConfirmCommand => new DelegateCommand(ChangeConfig);
        public ICommand AddConfigCommand => new DelegateCommand(AddConfig);
        public ICommand AddUserCommand => new DelegateCommand(AddUser);
        public ICommand AddPlaylistToSourceCommand => new DelegateCommand(AddPlaylistToSource);

        private void AddPlaylistToSource()
        {
            Config.SourcePlaylists.Add(new Playlist());
        }

        private void AddUser()
        {
            Config.Users.Add(FileHandler.SaveConfigAddUser(SelectedConfigPath));
        }

        private void AddConfig()
        {
            var path = Path.EndsWith("\\") ? Path : Path + "\\";
            var file = PathNewConfig.EndsWith(".json") ? path + PathNewConfig : path + PathNewConfig + ".json";
            using (File.Create(file))
            {
            }
            var json = JsonConvert.SerializeObject(new Config(), Formatting.Indented);
            File.WriteAllText(file, json);
            Config = new Config();
        }

        public void LoadExistingConfigsFromPath()
        {
            var path = Path;
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

        public void ChangeConfig()
        {
            FileHandler.SaveConfigEditUser(SelectedConfigPath, Config.Users);
            //TODO SaveCondigEdit für alle Optionen und Notifications aber müssen uns ein anders tool suchen verstehe das aktuelle nicht ganz
            var json = JsonConvert.SerializeObject(Config, Formatting.Indented);
            File.WriteAllText(SelectedConfigPath, json);
        }

        private void LoadConfigFromPath()
        {
            var config = FileHandler.LoadConfig(SelectedConfigPath);
            if (config == null)
                Path = _oldPath;
            Config = config;
        }
    }
}