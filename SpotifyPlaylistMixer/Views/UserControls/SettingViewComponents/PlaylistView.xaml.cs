using System.Windows;
using SpotifyPlaylistMixer.DataObjects;

namespace SpotifyPlaylistMixer.Views.UserControls.SettingViewComponents
{
    /// <summary>
    ///     Interaction logic for PlaylistView.xaml
    /// </summary>
    public partial class PlaylistView
    {
        public static readonly DependencyProperty DeleteButtonVisibilityProperty = DependencyProperty.Register(
            nameof(DeleteButtonVisibility), typeof(Visibility), typeof(PlaylistView),
            new PropertyMetadata(Visibility.Visible));

        public delegate void DeleteClickedEventHandler(PlaylistView sender, Playlist playlist);

        public PlaylistView()
        {
            InitializeComponent();
        }

        public event DeleteClickedEventHandler DeleteClicked;

        public Visibility DeleteButtonVisibility
        {
            get
            {
                var value = GetValue(DeleteButtonVisibilityProperty);
                if (value != null)
                    return (Visibility) value;
                return Visibility.Visible;
            }
            set => SetValue(DeleteButtonVisibilityProperty, value);
        }

        private void DeleteButton_OnClick(object sender, RoutedEventArgs e)
        {
            var playlist = DataContext as Playlist;
            DeleteClicked?.Invoke(this, playlist);
        }
    }
}