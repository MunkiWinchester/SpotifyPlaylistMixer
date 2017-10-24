using System;
using System.Globalization;
using System.Windows.Data;

namespace UserInterface
{
    public class SubtractIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (value == null || !int.TryParse(value.ToString(), out var parsedValue))
                return 0;
            if (parameter != null && int.TryParse(parameter.ToString(), out var parsedParameter))
                return parsedValue - parsedParameter;
            return parsedValue;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}