using System.ComponentModel;
using System.Windows;
using UserInterface.Properties;

namespace UserInterface.Views
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public new const string WindowTitle = "Spotify Mixer";

        public MainWindow()
        {
            InitializeComponent();
            Title = WindowTitle;

            Top = Settings.Default.Top;
            Left = Settings.Default.Left;
            Height = Settings.Default.Height;
            Width = Settings.Default.Width;
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            HamburgerMenuControl.SelectedIndex = 0;
        }

        /// <summary>
        /// Saves the position
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            Settings.Default.Top = Top;
            Settings.Default.Left = Left;
            Settings.Default.Height = Height;
            Settings.Default.Width = Width;
            Settings.Default.Save();
        }
    }
}