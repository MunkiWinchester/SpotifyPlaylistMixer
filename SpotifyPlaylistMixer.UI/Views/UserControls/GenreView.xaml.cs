using System;
using System.Windows.Controls;
using LiveCharts;

namespace SpotifyPlaylistMixer.UI.Views.UserControls
{
    /// <summary>
    /// Interaction logic for GenreView.xaml
    /// </summary>
    public partial class GenreView : UserControl
    {
        public GenreView()
        {
            InitializeComponent();

            // TODO: Remove this
            PointLabel = chartPoint =>
                $"{chartPoint.Y} ({chartPoint.Participation:P})";
            DataContext = this;
        }

        public Func<ChartPoint, string> PointLabel { get; set; }
    }
}
