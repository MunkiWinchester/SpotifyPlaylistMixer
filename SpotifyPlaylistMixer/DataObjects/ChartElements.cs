using System.Collections.Generic;
using LiveCharts;

namespace SpotifyPlaylistMixer.DataObjects
{
    public class ChartElements
    {
        public SeriesCollection SeriesCollection { get; set; }
        public List<ChartElement> Elements { get; set; }
    }
}