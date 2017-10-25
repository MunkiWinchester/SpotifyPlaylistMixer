using System.Windows;
using DataObjects.DataObjects;

namespace UserInterface.Views.UserControls.SettingViewComponents
{
    /// <summary>
    ///     Interaction logic for PlaylistView.xaml
    /// </summary>
    public partial class PlaylistView
    {
        public delegate void DeleteClickedEventHandler(PlaylistView sender, Playlist playlist);

        public static readonly DependencyProperty DeleteButtonVisibilityProperty = DependencyProperty.Register(
            nameof(DeleteButtonVisibility), typeof(Visibility), typeof(PlaylistView),
            new PropertyMetadata(Visibility.Visible));

        public PlaylistView()
        {
            InitializeComponent();
        }

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

        public event DeleteClickedEventHandler DeleteClicked;

        private void DeleteButton_OnClick(object sender, RoutedEventArgs e)
        {
            var playlist = DataContext as Playlist;
            DeleteClicked?.Invoke(this, playlist);
        }
    }
}