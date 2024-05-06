using CefSharp.Wpf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using nthLink.Wpf.ViewModels;

namespace AttachedProperties
{
    public static class LoadedUrl
    {
        public static bool GetIs(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsProperty);
        }

        public static void SetIs(DependencyObject obj, bool value)
        {
            obj.SetValue(IsProperty, value);
        }

        // Using a DependencyProperty as the backing store for Is.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsProperty =
            DependencyProperty.RegisterAttached("Is", typeof(bool), typeof(LoadedUrl), new PropertyMetadata(OnLoadedUrlPropertyChanged));

        private static void OnLoadedUrlPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ChromiumWebBrowser chromiumWebBrowser)
            {
                if (e.NewValue is bool b)
                {
                    if (b)
                    {
                        chromiumWebBrowser.AddHandler(ChromiumWebBrowser.LoadedEvent, ChromiumWebBrowser_Loaded);
                    }
                    else
                    {
                        chromiumWebBrowser.RemoveHandler(ChromiumWebBrowser.LoadedEvent, ChromiumWebBrowser_Loaded);
                    }
                }
            }
        }

        private static async void ChromiumWebBrowser_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is ChromiumWebBrowser chromiumWebBrowser &&
               chromiumWebBrowser.DataContext is WebItemViewModel webItemViewModel &&
                !string.IsNullOrEmpty(webItemViewModel.Url))
            {
                await chromiumWebBrowser.LoadUrlAsync(webItemViewModel.Url);
            }
        }
    }
}
