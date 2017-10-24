using DataObjects.DataObjects;
using UserInterface.ViewModels;

namespace UserInterface.Views.UserControls
{
    /// <summary>
    ///     Interaction logic for SettingView.xaml
    /// </summary>
    public partial class SettingView
    {
        public SettingView()
        {
            InitializeComponent();
        }

        private void Pl_OnDeleteClicked(SettingViewComponents.PlaylistView sender, Playlist playlist)
        {
            var settingViewModel = DataContext as SettingViewModel;
            settingViewModel?.DeletePlaylist(playlist);
        }
    }
}