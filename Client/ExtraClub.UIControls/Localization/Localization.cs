using System.Windows;
using System.Threading;

namespace ExtraClub.UIControls.Localization
{
    public static class Localization
    {
        public static Visibility RuVisibility
        {
            get
            {
                return IsRussian ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public static bool IsRussian
        {
            get
            {
                return Thread.CurrentThread.CurrentUICulture.IetfLanguageTag.ToLower() == "ru-ru";
            }
        }
    }
}
