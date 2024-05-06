using nthLink.Header.Interface;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace nthLink.Wpf.Model
{
    public class MainThreadSyncContext : IMainThreadSyncContext
    {
        public async Task Post(Action action)
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                action.Invoke();
            }
            else
            {
                await Application.Current.Dispatcher.BeginInvoke(action, null);
            }
        }
    }
}
