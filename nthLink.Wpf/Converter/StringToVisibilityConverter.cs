using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace nthLink.Wpf.Converter
{
    [ValueConversion(typeof(string), typeof(Visibility))]
    internal class StringToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is string str && !string.IsNullOrEmpty(str)
                ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
