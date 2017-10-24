using System.Collections.Generic;

namespace DataObjects.DataObjects
{
    public class ChartElement
    {
        public string Name { get; set; }
        public float PercentageValue { get; set; }
        public int Occurrences { get; set; }
        public List<PlaylistElement> OccurrenceIn { get; set; } = new List<PlaylistElement>();
    }
}