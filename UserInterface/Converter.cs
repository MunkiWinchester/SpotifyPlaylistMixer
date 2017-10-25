using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using WpfUtility.Services;

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

    public sealed class NegatedBooleanToVisibilityConverter : BooleanConverter<Visibility>
    {
        public NegatedBooleanToVisibilityConverter() :
            base(Visibility.Collapsed, Visibility.Visible)
        { }
    }

    public class SelectedItemToContentConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // first value is selected menu item, second value is selected option item
            if (values != null && values.Length > 1)
            {
                return values[0] ?? values[1];
            }
            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return targetTypes.Select(t => Binding.DoNothing).ToArray();
        }
    }
}