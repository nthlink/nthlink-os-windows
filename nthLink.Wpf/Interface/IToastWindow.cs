using System;

namespace nthLink.Wpf.Interface
{
    internal interface IToastWindow
    {
        void Show(object content, TimeSpan timeSpan);

        void Cancel();
    }
}
