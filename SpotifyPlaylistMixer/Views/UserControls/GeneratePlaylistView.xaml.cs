using System.IO;
using ReactiveUI;
using SpotifyPlaylistMixer.ViewModels;

namespace SpotifyPlaylistMixer.Views.UserControls
{
    /// <summary>
    /// Interaction logic for GeneratePlaylistView.xaml
    /// </summary>
    public partial class GeneratePlaylistView : IViewFor<GeneratePlaylistViewModel>
    {
        public GeneratePlaylistView()
        {
            ViewModel = new GeneratePlaylistViewModel();
            InitializeComponent();

            DataContext = ViewModel;

            var dir = Directory.GetCurrentDirectory();
            ViewModel.Path = $@"{dir}\Resources\Examples\Config\";
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (GeneratePlaylistViewModel)value;
        }

        public GeneratePlaylistViewModel ViewModel { get; set; }
    }
}
