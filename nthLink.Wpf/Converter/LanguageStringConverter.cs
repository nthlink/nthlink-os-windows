using nthLink.Header.Interface;
using System;
using System.Globalization;
using System.Windows.Data;

namespace nthLink.Wpf.Converter
{
    internal class LanguageStringConverter : IValueConverter
    {
        public static ILanguageService? LanguageService { get; internal set; }

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string key && LanguageService != null)
            {
                return LanguageService.GetString(key);
            }
            else
            {
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
