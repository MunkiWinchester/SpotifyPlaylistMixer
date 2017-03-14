using System.Windows;
using ReactiveUI;
using SpotifyPlaylistMixer.UI.ViewModels;

namespace SpotifyPlaylistMixer.UI.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IViewFor<SettingViewModel>
    {
        public MainWindow()
        {
            ViewModel = new SettingViewModel();
            InitializeComponent();

            DataContext = ViewModel;
            
            ViewModel.Path = @"Config.json";
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (SettingViewModel) value; }
        }

        public SettingViewModel ViewModel { get; set; }
    }
}
