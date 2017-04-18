using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using SpotifyPlaylistMixer.DataObjects;

namespace SpotifyPlaylistMixer.Converter
{
    public class ListStringToStringConverter : IValueConverter, IComparable
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ConnectedString(value);
        }

        public static string ConnectedString(object value)
        {
            var list = value as CustomList<string>;
            var connectedString = "";
            if (list != null && list.Any())
            {
                connectedString = list.Aggregate((s, next) => $"{s}; {next}");
            }
            return connectedString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("Two way conversion is not supported.");
        }

        public int CompareTo(object obj)
        {
            var otherValue = ConnectedString(obj);
            var baseValue = ConnectedString(this);
            return string.Compare(baseValue, otherValue, StringComparison.CurrentCulture);
        }
    }
}