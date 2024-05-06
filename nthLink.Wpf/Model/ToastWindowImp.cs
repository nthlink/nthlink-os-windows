using nthLink.Header.Interface;
using nthLink.Wpf.Interface;
using nthLink.Wpf.Views;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace nthLink.Wpf.Model
{
    internal class ToastWindowImp : IToastWindow
    {
        private ToastWindow? toastWindow;
        private readonly IMainThreadSyncContext mainThreadSyncContext;
        private CancellationTokenSource? cancellationTokenSource;

        private TimeSpan timeSpan = TimeSpan.MinValue;

        public ToastWindowImp(IMainThreadSyncContext mainThreadSyncContext)
        {
            this.mainThreadSyncContext = mainThreadSyncContext;
        }

        public void Show(object content, TimeSpan timeSpan)
        {
            this.mainThreadSyncContext.Post(() =>
            {
                if (this.toastWindow != null)
                {
                    return;
                }

                this.timeSpan = timeSpan;
                this.toastWindow = MakeNewToastWindow();

                this.toastWindow.ToastContent = content;

                if (content is ICanLoad canLoad)
                {
                    canLoad.Loaded += CanLoad_Loaded;
                }
                else
                {
                    DelayClose();
                }

                this.toastWindow.Show();
            });

            ToastWindow MakeNewToastWindow()
            {
                ToastWindow toastWindow = new ToastWindow();
                toastWindow.Left = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width - 20 - toastWindow.Width;
                toastWindow.Top = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height - 10 - toastWindow.Height;
                toastWindow.Opacity = 0.001;
                toastWindow.IsHitTestVisible = false;
                return toastWindow;
            }
        }

        private void CanLoad_Loaded(ICanLoad canLoad)
        {
            canLoad.Loaded -= CanLoad_Loaded;
            DelayClose();
        }
        private void DelayClose()
        {
            if (this.toastWindow == null)
            {
                return;
            }

            if (this.toastWindow.Opacity != 1)
            {
                this.toastWindow.BeginAnimation(Window.OpacityProperty,
                    new DoubleAnimation()
                    {
                        Duration = TimeSpan.FromMilliseconds(300),
                        To = 1,
                    });
            }

            this.toastWindow.IsHitTestVisible = true;

            this.cancellationTokenSource = new CancellationTokenSource();

            Task.Delay(this.timeSpan, this.cancellationTokenSource.Token).ContinueWith(task =>
            {
                this.mainThreadSyncContext.Post(() =>
                {
                    if (this.toastWindow != null)
                    {
                        this.toastWindow.Close();
                        this.toastWindow.ToastContent = null;
                        this.toastWindow = null;
                    }
                    this.cancellationTokenSource = null;
                    this.timeSpan = TimeSpan.MinValue;
                });
            });
        }

        public void Cancel()
        {
            if (this.cancellationTokenSource != null)
            {
                this.cancellationTokenSource.Cancel();
            }
        }
    }
}
