using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SpotifyPlaylistMixer.Views.UserControls
{
    /// <summary>
    /// Interaction logic for AddButtonView.xaml
    /// </summary>
    public partial class AddButtonView : UserControl
    {
        public AddButtonView()
        {
            InitializeComponent();
        }
        public ICommand ClickCommand
        {
            get { return (ICommand)GetValue(ClickCommandProperty); }
            set { SetValue(ClickCommandProperty, value); }
        }

        public static readonly DependencyProperty ClickCommandProperty =
            DependencyProperty.Register("ClickCommand", typeof(ICommand), typeof(AddButtonView), new UIPropertyMetadata(null));
    }
}
