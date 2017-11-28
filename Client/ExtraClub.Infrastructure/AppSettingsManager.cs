using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Microsoft.Win32;

namespace ExtraClub.Infrastructure
{
    public static class AppSettingsManager
    {
        public static string GetSetting(string settingName)
        {
#if BEAUTINIKA
            var readKey = Registry.CurrentUser.OpenSubKey("software\\beautinika");
#else
            var readKey = Registry.CurrentUser.OpenSubKey("software\\Extra");
#endif
            if (readKey != null)
            {
                var res = readKey.GetValue(settingName) as string;
                readKey.Close();
                if (res != null) return res;
            }
            var res1 = ConfigurationManager.AppSettings.Get(settingName);
            if (res1 != null)
            {
                SetSetting(settingName, res1);
            }
            else return null;
            return GetSetting(settingName);
        }

        public static void SetSetting(string settingName, string settingValue)
        {
#if BEAUTINIKA
            RegistryKey saveKey = Registry.CurrentUser.CreateSubKey("software\\beautinika");
#else
            RegistryKey saveKey = Registry.CurrentUser.CreateSubKey("software\\Extra");
#endif
            saveKey.SetValue(settingName, settingValue);
            saveKey.Close();
        }

        static Dictionary<string, bool> Cached = new Dictionary<string, bool>();

        public static bool GetSettingCached(string settingName)
        {
            if (Cached.ContainsKey(settingName))
            {
                return Cached[settingName];
            }

#if BEAUTINIKA
            var readKey = Registry.CurrentUser.OpenSubKey("software\\beautinika");
#else
            var readKey = Registry.CurrentUser.OpenSubKey("software\\Extra");
#endif
            if (readKey != null)
            {
                var res = readKey.GetValue(settingName) as string;
                readKey.Close();
                if (!Cached.ContainsKey(settingName))
                {
                    Cached[settingName] = res == "1";
                }
                if (res != null) return res == "1";
            }
            var res1 = ConfigurationManager.AppSettings.Get(settingName);
            if (res1 != null)
            {
                SetSetting(settingName, "1");
            }
            else return false;
            return GetSetting(settingName) == "1";
        }

        public static void FlushCache()
        {
            Cached.Clear();
        }
    }
}
