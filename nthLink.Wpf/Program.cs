using nthLink.Header;
using nthLink.Header.Enum;
using nthLink.SDK.Extension;
using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace nthLink.Wpf
{
    class Program
    {
        /// <summary>
        /// Entry Point.
        /// </summary>
        [STAThread]
        public static int Main(string[] args)
        {
            EventWaitHandle singleInstanceLock =
                new EventWaitHandle(false, EventResetMode.AutoReset, Const.String.SingleInstanceLock, out bool isSingle);

            if (!isSingle)
            {
                singleInstanceLock.Set();
                return (int)ErrorCodeEnum.AppAlreadyOpened;
            }

            try
            {
                new App().Run();
            }
            catch (Exception e)
            {
                File.WriteAllText($"exception_pid{Process.GetCurrentProcess().Id}_{DateTime.Now:yyyyMMdd_HHmmss_fff}.txt", e.ToDisplay());
            }

            return 0;
        }
    }
}
