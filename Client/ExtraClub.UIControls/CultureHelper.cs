using System.Globalization;
using Telerik.Windows.Controls;
using ExtraClub.Infrastructure;

namespace ExtraClub.UIControls
{
    public static class CultureHelper
    {
        static CultureHelper()
        {
            LocalizationManager.Manager = new ExtraClub.UIControls.Localization.CustomLocalizationManager();
            var lng = AppSettingsManager.GetSetting("Language");
            if (lng == "0")
            {
                ci = System.Globalization.CultureInfo.GetCultureInfo("ru-RU").Clone() as CultureInfo;
            }
            else if (lng == "1")
            {
                ci = System.Globalization.CultureInfo.GetCultureInfo("en-US").Clone() as CultureInfo;
                ci.NumberFormat.CurrencySymbol = "€";
                ci.NumberFormat.CurrencyPositivePattern = 2;
                ci.NumberFormat.CurrencyNegativePattern = 2;
            }
            else if(lng == "3")
            {
                ci = System.Globalization.CultureInfo.GetCultureInfo("ru-RU").Clone() as CultureInfo;
                ci.NumberFormat.CurrencyPositivePattern = 3;
                ci.NumberFormat.CurrencyNegativePattern = 3;
                ci.NumberFormat.CurrencySymbol = "сум.";
            }
            else
            {
                ci = System.Globalization.CultureInfo.GetCultureInfo("ru-RU").Clone() as CultureInfo;
                ci.NumberFormat.CurrencyPositivePattern = 3;
                ci.NumberFormat.CurrencyNegativePattern = 3;
                ci.NumberFormat.CurrencySymbol = "тг.";
            }
        }

        static CultureInfo ci;

        public static void FixupCulture()
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = ci;
            System.Threading.Thread.CurrentThread.CurrentCulture = ci;
            LocalizationManager.DefaultCulture = ci;
        }
    }
}
