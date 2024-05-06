using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace nthLink.Wpf.MarkupExtension
{
    public class ObjectExtension : System.Windows.Markup.MarkupExtension
    {
        private readonly object obj;

        public IValueConverter? Converter { get; set; }

        public object? ConverterParameter { get; set; }

        public ObjectExtension(object obj)
        {
            this.obj = obj;
        }

        public override object? ProvideValue(IServiceProvider serviceProvider)
        {
            if (Converter == null)
            {
                return this.obj;
            }
            else
            {
                if (serviceProvider.GetService(typeof(IProvideValueTarget)) is IProvideValueTarget provideValueTarget)
                {
                    return Converter.Convert(this.obj, provideValueTarget.TargetObject.GetType(), ConverterParameter, CultureInfo.CurrentUICulture);
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
