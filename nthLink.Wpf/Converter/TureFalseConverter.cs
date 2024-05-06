using System;
using System.Globalization;
using System.Windows.Data;

namespace nthLink.Wpf.Converter
{
    [ValueConversion(typeof(bool), typeof(object))]
	public class TureFalseConverter : IValueConverter
	{
		public object? True { get; set; }
		public object? False { get; set; }
		public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
			{
				return False;
			}
			else if (value is bool b)
			{
				return b ? True : False;
			}
			else if (value is bool?)
			{
				bool? nulb = (bool?)value;

				return nulb.HasValue && nulb.Value ? True : False;
			}
			else if (value is string s)
			{
				return string.IsNullOrEmpty(s) ? False : True;
			}
			else
			{
				return value.Equals(0) ? False : True;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
