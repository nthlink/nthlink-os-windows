using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using nthLink.Header.Enum;

namespace nthLink.Wpf.Converter
{
    internal class ServerStateStringColorConverter : IValueConverter
    {
        private readonly SolidColorBrush defaultBrush = new SolidColorBrush(Color.FromRgb(25, 232, 224));
        private readonly SolidColorBrush terminatingBrush = new SolidColorBrush(Color.FromRgb(236, 91, 97));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is StateEnum state)
            {
                if (state == StateEnum.Terminating)
                {
                    return terminatingBrush;
                }
            }

            return defaultBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
