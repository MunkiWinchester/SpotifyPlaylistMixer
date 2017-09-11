using System.IO;
using System.Windows;
using SpotifyPlaylistMixer.ViewModels;

namespace SpotifyPlaylistMixer.Views.UserControls
{
    /// <summary>
    ///     Interaction logic for PlaylistView.xaml
    /// </summary>
    public partial class PlaylistView
    {
        private readonly PlaylistViewModel ViewModel;

        public PlaylistView()
        {
            ViewModel = new PlaylistViewModel();
            InitializeComponent();

            DataContext = ViewModel;
        }

        private void PlaylistView_OnLoaded(object sender, RoutedEventArgs e)
        {
            var dir = Directory.GetCurrentDirectory();
            ViewModel.LoadExistingPlaylistsFromPath($@"{dir}\Resources\Examples\");
        }
    }
}