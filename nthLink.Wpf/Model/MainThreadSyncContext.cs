using nthLink.Header.Interface;
using System;
using System.Threading.Tasks;

namespace nthLink.Wpf.Model
{
    public class MainThreadSyncContext : IMainThreadSyncContext
    {
        public async Task Post(Action action)
        {
            if (System.Windows.Application.Current.Dispatcher.CheckAccess())
            {
                action.Invoke();
            }
            else
            {
                await System.Windows.Application.Current.Dispatcher.BeginInvoke(action, null);
            }
        }
    }
}
