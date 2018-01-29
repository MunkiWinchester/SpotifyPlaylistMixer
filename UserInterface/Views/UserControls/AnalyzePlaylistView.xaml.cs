using System.Windows;
using System.Windows.Controls;
using UserInterface.ViewModels;

namespace UserInterface.Views.UserControls
{
    /// <summary>
    /// Interaction logic for AnalyzePlaylistView.xaml
    /// </summary>
    public partial class AnalyzePlaylistView : UserControl
    {
        public AnalyzePlaylistView()
        {
            InitializeComponent();
        }

        private void AnalyzePlaylistView_OnLoaded(object sender, RoutedEventArgs e)
        {
            if(DataContext is AnalyzePlaylistViewModel viewModel)
                viewModel.AuthenticateSpotify();
        }
    }
}
