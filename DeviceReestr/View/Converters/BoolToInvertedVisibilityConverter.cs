using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DeviceReestr.View.Converters
{
    public class BoolToInvertedVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var collapse = true;
            if (parameter != null)
            {
                if ((parameter is int && ((int)parameter) > 0) ||
                    ((parameter is string) && string.Compare(parameter as string, "hide", StringComparison.InvariantCultureIgnoreCase) == 0))
                    collapse = false;
            }

            return (bool)value ? (collapse ? Visibility.Collapsed : Visibility.Hidden) : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((Visibility)value) != Visibility.Visible;
        }
    }
}