using System.Linq;
using System.Reactive.Linq;
using ReactiveUI;
using SpotifyPlaylistMixer.Business;
using SpotifyPlaylistMixer.DataObjects;
using ReactiveCommand = ReactiveUI.ReactiveCommand;

namespace SpotifyPlaylistMixer.UI.ViewModels
{
    public class SettingViewModel : ReactiveObject
    {
        private string _path;
        public string Path
        {
            get { return _path; }
            set { this.RaiseAndSetIfChanged(ref _path, value); }
        }

        public ReactiveCommand<string, Config> LoadConfigCommand { get; protected set; }
        private readonly ObservableAsPropertyHelper<Config> _config;
        public Config Config => _config.Value;

        public ReactiveCommand ConfirmCommand { get; private set; }

        public SettingViewModel()
        {
            LoadConfigCommand = ReactiveCommand.Create<string, Config>(LoadConfigFromPath);
            this.WhenAnyValue(x => x.Path)
                .Select(x => x?.Trim())
                .DistinctUntilChanged(x => x)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .InvokeCommand(LoadConfigCommand);
            _config = LoadConfigCommand.ToProperty(this, x => x.Config, new Config());


            var canConfirmConfigObservable = this.WhenAny(vm => vm.Config,
                s =>
                    !string.IsNullOrEmpty(s.Value.TargetPlaylist?.Identifier)
                    && !string.IsNullOrEmpty(s.Value.TargetPlaylist.Owner.Identifier)
                    && s.Value.SourcePlaylists.Any()
                    && !string.IsNullOrEmpty(s.Value.SourcePlaylists.FirstOrDefault()?.Name)
                    && !string.IsNullOrEmpty(s.Value.SourcePlaylists.FirstOrDefault()?.Owner.Identifier)
                );
            ConfirmCommand = ReactiveCommand.Create(ChangeConfig, canConfirmConfigObservable);
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