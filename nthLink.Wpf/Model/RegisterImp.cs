using Microsoft.Win32;
using nthLink.Header;
using nthLink.Wpf.Interface;
using System;

namespace nthLink.Wpf.Model
{
    class RegisterImp : IWindowsRegister
    {
        private readonly static string path = $"SOFTWARE\\{Const.String.ProductName}";

        public event Action<string, string>? RegisterChanged;

        public string GetRegister(string key)
        {
            RegistryKey? keyObj = Registry.LocalMachine.OpenSubKey($"{path}");

            if (keyObj != null && keyObj.GetValue(key) is string str)
            {
                return str;
            }
            else
            {
                return string.Empty;
            }
        }

        public bool SetRegister(string key, string value)
        {
            RegistryKey? keyObj = Registry.LocalMachine.OpenSubKey($"{path}", true);

            if (keyObj == null)
            {
                RegistryKey? newKey = Registry.LocalMachine.CreateSubKey($"{path}");

                if (newKey == null)
                {
                    return false;
                }
                else
                {
                    return SetRegister(key, value);
                }
            }
            else
            {
                keyObj.SetValue(key, value);

                if (RegisterChanged != null)
                {
                    RegisterChanged.Invoke(key, value);
                }

                return true;
            }
        }
    }
}
