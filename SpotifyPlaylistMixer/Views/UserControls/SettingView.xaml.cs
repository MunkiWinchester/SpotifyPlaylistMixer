using System.IO;
using ReactiveUI;
using SpotifyPlaylistMixer.ViewModels;

namespace SpotifyPlaylistMixer.Views.UserControls
{
    /// <summary>
    ///     Interaction logic for SettingView.xaml
    /// </summary>
    public partial class SettingView : IViewFor<SettingViewModel>
    {
        public SettingView()
        {
            ViewModel = new SettingViewModel();
            InitializeComponent();

            DataContext = ViewModel;

            var dir = Directory.GetCurrentDirectory();
            ViewModel.Path = $@"{dir}\Resources\Examples\Config\";
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (SettingViewModel) value;
        }

        public SettingViewModel ViewModel { get; set; }
    }
}