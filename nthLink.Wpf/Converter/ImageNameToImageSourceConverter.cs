using nthLink.SDK.Extension;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace nthLink.Wpf.Converter
{
    [ValueConversion(typeof(object), typeof(ImageSource))]
    public class ImageNameToImageSourceConverter : IValueConverter
    {
        private string? path;
        public string? Path
        {
            get => this.path;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    return;
                }
                if (value.EndsWith("."))
                {
                    this.path = value.Substring(0, value.Length - 2);
                }
                else
                {
                    this.path = value;
                }
            }
        }
        private string? extension;
        public string? Extension
        {
            get => this.extension;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    return;
                }

                if (value.StartsWith("."))
                {
                    this.extension = value.Substring(1);
                }
                else
                {
                    this.extension = value;
                }
            }
        }

        public object? Header { get; set; }

        private static readonly Dictionary<string, BitmapImage> keyValuePairs = new Dictionary<string, BitmapImage>();
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            string realPath;

            if (value is string str)
            {
                realPath = $"{Path}.{GetImageName(Header, str, parameter)}.{Extension}";
            }
            else
            {
                string name = GetImageName(Header, value, parameter);

                if (string.IsNullOrEmpty(name))
                {
                    return null;
                }

                realPath = $"{Path}.{name}.{Extension}";
            }

            if (!keyValuePairs.ContainsKey(realPath))
            {
                using (Stream? stream = typeof(App).Assembly.GetManifestResourceStream($"{nameof(nthLink)}.{nameof(nthLink.Wpf)}.{realPath}"))
                {
                    if (stream != null)
                    {
                        BitmapImage bitmapImage = new BitmapImage();

                        bitmapImage.BeginInit();
                        bitmapImage.StreamSource = stream;
                        bitmapImage.EndInit();

                        keyValuePairs.Add(realPath, bitmapImage);
                    }
                }
            }

            return keyValuePairs.ContainsKey(realPath) ? keyValuePairs[realPath] : null;
        }

        protected virtual string GetImageName(object? header, object? input, object? parameter)
        {
            return $"{header.ToStr()}{input.ToStr()}{parameter.ToStr()}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
