using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using Newtonsoft.Json;
using ReactiveUI;
using SpotifyPlaylistMixer.DataObjects;

namespace SpotifyPlaylistMixer.ViewModels
{
    public class GenreViewModel : ReactiveObject
    {
        private string _path;

        public string Path
        {
            get { return _path; }
            set { this.RaiseAndSetIfChanged(ref _path, value); }
        }

        private string _selectedPlaylistPath;

        public string SelectedPlaylistPath
        {
            get { return _selectedPlaylistPath; }
            set { this.RaiseAndSetIfChanged(ref _selectedPlaylistPath, value); }
        }
        
        public ReactiveCommand<string, List<PlaylistElementReal>> LoadExistingPlaylistCommand
        {
            get;
            protected set;
        }
        private readonly ObservableAsPropertyHelper<List<PlaylistElementReal>> _existingPlaylist;
        public List<PlaylistElementReal> ExistingPlaylist => _existingPlaylist.Value;

        public ReactiveCommand<string, List<string>> LoadExistingPlaylists { get; protected set; }
        private readonly ObservableAsPropertyHelper<List<string>> _existingPlaylists;
        public List<string> ExistingPlaylists => _existingPlaylists.Value;
        

        public GenreViewModel()
        {
            LoadExistingPlaylistCommand =
                ReactiveCommand.Create<string, List<PlaylistElementReal>>(LoadExistingPlaylistFromPath);
            this.WhenAnyValue(x => x.SelectedPlaylistPath)
                .Select(x => x?.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .InvokeCommand(LoadExistingPlaylistCommand);
            _existingPlaylist = LoadExistingPlaylistCommand.ToProperty(this, x => x.ExistingPlaylist,
                new List<PlaylistElementReal>());

            LoadExistingPlaylistCommand.Subscribe(result =>
            {
                if (ExistingPlaylist != null && ExistingPlaylist.Any())
                {
                }
            });

            LoadExistingPlaylists = ReactiveCommand.Create<string, List<string>>(LoadExistingPlaylistsFromPath);
            this.WhenAnyValue(x => x.Path)
                .Select(x => x?.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .InvokeCommand(LoadExistingPlaylists);
            _existingPlaylists = LoadExistingPlaylists.ToProperty(this, x => x.ExistingPlaylists, new List<string>());
        }

        private List<PlaylistElementReal> LoadExistingPlaylistFromPath(string path)
        {
            var elements = JsonConvert.DeserializeObject<IEnumerable<PlaylistElementReal>>(
                File.ReadAllText(path)).ToList();
            return elements;
        }

        private List<string> LoadExistingPlaylistsFromPath(string path)
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
    }
}
