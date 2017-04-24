using System.IO;
using ReactiveUI;
using SpotifyPlaylistMixer.ViewModels;

namespace SpotifyPlaylistMixer.Views.UserControls
{
    /// <summary>
    /// Interaction logic for DetailsGridView.xaml
    /// </summary>
    public partial class DetailsGridView : IViewFor<DetailsGridViewModel>
    {
        public DetailsGridView()
        {
            ViewModel = new DetailsGridViewModel();
            InitializeComponent();

            DataContext = ViewModel;
            
            var dir = Directory.GetCurrentDirectory();
            ViewModel.Path = $@"{dir}\Resources\Examples\";
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (DetailsGridViewModel)value;
        }

        public DetailsGridViewModel ViewModel { get; set; }
    }
}
