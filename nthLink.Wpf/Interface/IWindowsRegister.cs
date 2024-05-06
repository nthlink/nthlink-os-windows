using System;

namespace nthLink.Wpf.Interface
{
    interface IWindowsRegister
    {
        event Action<string, string> RegisterChanged;
        bool SetRegister(string key, string value);
        string GetRegister(string key);
    }
}
