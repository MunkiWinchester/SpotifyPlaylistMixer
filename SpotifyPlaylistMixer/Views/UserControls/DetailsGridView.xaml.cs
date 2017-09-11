using System.IO;
using System.Windows;
using SpotifyPlaylistMixer.ViewModels;

namespace SpotifyPlaylistMixer.Views.UserControls
{
    /// <summary>
    ///     Interaction logic for DetailsGridView.xaml
    /// </summary>
    public partial class DetailsGridView
    {
        public DetailsGridView()
        {
            ViewModel = new DetailsGridViewModel();
            InitializeComponent();

            DataContext = ViewModel;
        }

        public DetailsGridViewModel ViewModel { get; set; }

        private void DetailsGridView_OnLoaded(object sender, RoutedEventArgs e)
        {
            var dir = Directory.GetCurrentDirectory();
            ViewModel.LoadExistingPlaylistsFromPath($@"{dir}\Resources\Examples\");
        }
    }
}