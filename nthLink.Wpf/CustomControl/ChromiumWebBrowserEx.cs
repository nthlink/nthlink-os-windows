using CefSharp;
using CefSharp.Wpf;
using nthLink.Wpf.ViewModels;
using System;
using System.Threading.Tasks;
using System.Windows;
using nthLink.SDK.Model;

namespace nthLink.Wpf.CustomControl
{
    internal class ChromiumWebBrowserEx : ChromiumWebBrowser
    {
        private DelayAction? errorRetryDelay;
        private volatile bool isErrorRetryScheduled;

        public int ResearchMillisecond
        {
            get { return (int)GetValue(ResearchMillisecondProperty); }
            set { SetValue(ResearchMillisecondProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ResearchMillisecond.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ResearchMillisecondProperty =
            DependencyProperty.Register(nameof(ResearchMillisecond), typeof(int), typeof(ChromiumWebBrowserEx),
                new PropertyMetadata(0, OnResearchMillisecondChanged));

        private static void OnResearchMillisecondChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ChromiumWebBrowserEx control)
            {
                if (e.OldValue is int oldValue && oldValue == 0 &&
                    e.NewValue is int newValue && newValue > 0)
                {
                    Task.Run(control.AutoResearch);
                }
            }
        }

        public ChromiumWebBrowserEx()
        {
            Loaded += ChromiumWebBrowserEx_Loaded;

            FrameLoadEnd += OnBrowserFrameLoadEnd;

            LoadingStateChanged += ChromiumWebBrowserEx_LoadingStateChanged;

            LoadError += ChromiumWebBrowserEx_LoadError;

            Unloaded += ChromiumWebBrowserEx_Unloaded;

            // 使用 DelayAction 進行錯誤重試（5 秒）
            this.errorRetryDelay = new DelayAction(DoErrorRetry)
            {
                DelayMilliseconds = 5000
            };
        }

        private void ChromiumWebBrowserEx_Unloaded(object sender, RoutedEventArgs e)
        {
            this.errorRetryDelay?.CancelAction();
            this.isErrorRetryScheduled = false;
            this.GetBrowser()?.CloseBrowser(true);
            this.Dispose();
        }

        private async void ChromiumWebBrowserEx_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is WebViewModel webViewModel)
            {
                webViewModel.PropertyChanged += WebViewModel_PropertyChanged;
                await NavigateTo(webViewModel.Url);
            }
        }

        private async Task NavigateTo(string? url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                await Cef.GetGlobalCookieManager().DeleteCookiesAsync();

                await LoadUrlAsync(url);
            }
        }

        private async void WebViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(WebViewModel.Url))
            {
                if (sender is WebViewModel webViewModel)
                {
                    await NavigateTo(webViewModel.Url);
                }
            }
        }

        private void OnBrowserFrameLoadEnd(object? sender, FrameLoadEndEventArgs args)
        {
            if (args.Frame.IsMain)
            {
                // 一旦主框架成功完成載入，停止錯誤重試排程
                this.errorRetryDelay?.CancelAction();
                this.isErrorRetryScheduled = false;
                args.Browser
                    .MainFrame
                    .ExecuteJavaScriptAsync(
                    "document.body.style.overflow = 'hidden'");

                args.Browser
                   .MainFrame
                   .ExecuteJavaScriptAsync(
                   "document.body.style.zoom = '50%'");
            }
        }

        private void ChromiumWebBrowserEx_LoadError(object? sender, LoadErrorEventArgs e)
        {
            // 只處理主框架錯誤
            if (e.Frame.IsMain)
            {
                // 5 秒後重試載入（若期間重複觸發，會重新排程）
                this.isErrorRetryScheduled = true;
                this.errorRetryDelay?.DoAction();
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

        private async Task AutoResearch()
        {
            while (true)
            {
                int researchMillisecond = 0;
                string? url = string.Empty;

                Dispatcher.Invoke(() =>
                {
                    researchMillisecond = ResearchMillisecond;

                    if (this.DataContext is WebViewModel webViewModel)
                    {
                        url = webViewModel.Url;
                    }
                });

                if (researchMillisecond == 0)
                {
                    break;
                }

                await Task.Delay(researchMillisecond);

                // 若目前已有錯誤重試排程，略過本次週期刷新，避免衝突
                if (this.isErrorRetryScheduled)
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(url))
                {
                    await NavigateTo(url);
                }
            }
        }

        private void DoErrorRetry()
        {
            try
            {
                string? urlToLoad = null;
                Dispatcher.Invoke(() =>
                {
                    // 以 DataContext 上的最新 Url 為準
                    if (this.DataContext is WebViewModel vm && !string.IsNullOrEmpty(vm.Url))
                    {
                        urlToLoad = vm.Url;
                    }
                });

                if (!string.IsNullOrEmpty(urlToLoad))
                {
                    Dispatcher.InvokeAsync(async () =>
                    {
                        await LoadUrlAsync(urlToLoad);
                    });
                }

                // 單次延遲已觸發，清除旗標，讓週期刷新恢復作用（若仍錯誤會再次排程）
                this.isErrorRetryScheduled = false;
            }
            catch
            {
                // 忽略背景觸發例外
            }
        }
    }
}
