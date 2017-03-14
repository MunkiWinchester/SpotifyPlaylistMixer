using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LiveCharts;

namespace SpotifyPlaylistMixer.UI.UserControls
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
