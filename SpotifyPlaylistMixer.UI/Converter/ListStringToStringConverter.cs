using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace SpotifyPlaylistMixer.UI.Converter
{
    public class ListStringToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var list = value as List<string>;
            var connectedString = "";
            if (list != null)
            {
                connectedString = list.Aggregate((s, next) => $"{s}; {next}");
            }
            return connectedString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("Two way conversion is not supported.");
        }
    }
}