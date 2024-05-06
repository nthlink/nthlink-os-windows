using CefSharp;
using CefSharp.Wpf;
using System.Windows;
using nthLink.Wpf.ViewModels;
using System.Threading.Tasks;

namespace nthLink.Wpf.CustomControl
{
    internal class ChromiumWebBrowserEx : ChromiumWebBrowser
    {
        public ChromiumWebBrowserEx()
        {
            Loaded += ChromiumWebBrowserEx_Loaded;

            FrameLoadEnd += OnBrowserFrameLoadEnd;

            LoadingStateChanged += ChromiumWebBrowserEx_LoadingStateChanged;
        }

        private async void ChromiumWebBrowserEx_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is WebViewModel webViewModel)
            {
                webViewModel.PropertyChanged += WebViewModel_PropertyChanged;

                await NavigateTo(webViewModel);
            }
        }

        private async Task NavigateTo(WebViewModel webViewModel)
        {
            if (!string.IsNullOrEmpty(webViewModel.Url))
            {
                await Cef.GetGlobalCookieManager().DeleteCookiesAsync();

                await LoadUrlAsync(webViewModel.Url);
            }
        }

        private async void WebViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(WebViewModel.Url))
            {
                if (sender is WebViewModel webViewModel)
                {
                    await NavigateTo(webViewModel);
                }
            }
        }

        private void OnBrowserFrameLoadEnd(object? sender, FrameLoadEndEventArgs args)
        {
            if (args.Frame.IsMain)
            {
                args.Browser
                    .MainFrame
                    .ExecuteJavaScriptAsync(
                    "document.body.style.overflow = 'hidden'");
            }
        }

        private void ChromiumWebBrowserEx_LoadingStateChanged(object? sender, CefSharp.LoadingStateChangedEventArgs e)
        {
            if (e.IsLoading)
            {
                Dispatcher.Invoke(() =>
                {
                    if (this.DataContext is WebViewModel webViewModel)
                    {
                        webViewModel.RaiseLoadedEvent();
                    }
                });
            }
        }
    }
}
