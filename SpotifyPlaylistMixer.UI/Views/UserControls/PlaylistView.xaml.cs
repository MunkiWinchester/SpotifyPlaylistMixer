using ReactiveUI;
using SpotifyPlaylistMixer.UI.ViewModels;

namespace SpotifyPlaylistMixer.UI.Views.UserControls
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

            ViewModel.Path = @"N:\EDV\IT-ERP - Intern\ERP Mix der Woche";
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (PlaylistViewModel)value; }
        }

        public PlaylistViewModel ViewModel { get; set; }
    }
}
