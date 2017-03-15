using ReactiveUI;
using SpotifyPlaylistMixer.UI.ViewModels;

namespace SpotifyPlaylistMixer.UI.Views.UserControls
{
    /// <summary>
    /// Interaction logic for SettingView.xaml
    /// </summary>
    public partial class SettingView : IViewFor<SettingViewModel>
    {
        public SettingView()
        {
            ViewModel = new SettingViewModel();
            InitializeComponent();

            DataContext = ViewModel;

            ViewModel.Path = @"N:\EDV\IT-ERP - Intern\ERP Mix der Woche\Config\Config.json";
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (SettingViewModel)value; }
        }

        public SettingViewModel ViewModel { get; set; }
    }
}
