using nthLink.Header.Interface;
using nthLink.SDK.Extension;
using nthLink.SDK.Model;
using System.Windows.Data;

namespace nthLink.Wpf.MarkupExtension
{
    public class StrExtension : Binding
    {
        public StrExtension(string key) : base($"[{key}]")
        {
            this.Mode = BindingMode.OneWay;
            this.Source = App.ContainerProvider.Resolve<TranslationSource>().Unwrap();
        }
    }

    class TranslationSource : NotifyPropertyChangedBase
    {
        private ILanguageService languageService;

        public TranslationSource(ILanguageService languageService)
        {
            this.languageService = languageService;
        }

        public string this[string key]
        {
            get
            {
                string result = this.languageService.GetString(key);

                return string.IsNullOrEmpty(result) ?
                    $"Language key {key} not found." : result;
            }
        }
    }
}
