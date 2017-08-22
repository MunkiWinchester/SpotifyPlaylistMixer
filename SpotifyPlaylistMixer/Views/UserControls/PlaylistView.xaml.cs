using System.IO;
using ReactiveUI;
using SpotifyPlaylistMixer.ViewModels;

namespace SpotifyPlaylistMixer.Views.UserControls
{
    /// <summary>
    /// Interaction logic for PlaylistView.xaml
    /// </summary>
    public partial class PlaylistView : IViewFor<PlaylistViewModel>
    {
        public PlaylistView()
        {
            ViewModel = new PlaylistViewModel();
            InitializeComponent();

            DataContext = ViewModel;

            var dir = Directory.GetCurrentDirectory();
            ViewModel.Path = $@"{dir}\Resources\Examples\";
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (PlaylistViewModel)value;
        }

        public PlaylistViewModel ViewModel { get; set; }
    }
}
