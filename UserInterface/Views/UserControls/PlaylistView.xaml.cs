using System.IO;
using System.Windows;
using UserInterface.ViewModels;

namespace UserInterface.Views.UserControls
{
    /// <summary>
    ///     Interaction logic for PlaylistView.xaml
    /// </summary>
    public partial class PlaylistView
    {
        private readonly PlaylistViewModel _viewModel;

        public PlaylistView()
        {
            _viewModel = new PlaylistViewModel();
            InitializeComponent();

            DataContext = _viewModel;
        }

        private void PlaylistView_OnLoaded(object sender, RoutedEventArgs e)
        {
            var dir = Directory.GetCurrentDirectory();
            _viewModel.LoadExistingPlaylistsFromPath($@"{dir}\Resources\Examples\");
        }
    }
}