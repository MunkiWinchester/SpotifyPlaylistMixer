using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Business.Business;
using DataObjects.DataObjects;
using WpfUtility.Services;

namespace UserInterface.ViewModels
{
    public class SettingViewModel : ObservableObject
    {
        private Config _config;
        private ObservableCollection<KeyValuePair<string, string>> _existingConfigs;
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
        public ICommand DeleteUserCommand => new RelayCommand<User>(DeleteUser);

        public ICommand AddPlaylistToSourceCommand => new DelegateCommand(AddPlaylistToSource);

        private void DeleteUser(User user)
        {
            var result = Config.Users.Remove(user);
        }

        public void DeletePlaylist(Playlist playlist)
        {
            var result = Config.SourcePlaylists.Remove(playlist);
        }

        private void AddPlaylistToSource()
        {
            Config.SourcePlaylists.Add(new Playlist());
        }

        private void AddUser()
        {
            Config.Users.Add(new User());
        }

        private void AddConfig()
        {
            Config = new Config();
            FileHandler.SaveConfig(Config, Path, PathNewConfig);
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
            //TODO SaveCondigEdit für alle Optionen und Notifications aber müssen uns ein anders tool suchen verstehe das aktuelle nicht ganz
            FileHandler.SaveConfig(Config, SelectedConfigPath);
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