using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace TonusClub.CustomerPortal.Business
{
    public static class SmsCore
    {
        internal static void SendSms(string phone, string text)
        {
            phone = "79531731680";
            var wc = new WebClient();
            //wc.DownloadString(new Uri(String.Format(
            //    "http://gateway.api.sc/get/send_xml.php?user={0}&pwd={1}&sadr={2}&dadr={3}&text={4}",
            //    "tonusclub", //ConfigurationManager.AppSettings.Get("SmsLogin"),
            //    "09tonusclub12", //ConfigurationManager.AppSettings.Get("SmsPassword"),
            //    "TONUSCLUB", //ConfigurationManager.AppSettings.Get("SmsSenderName"),
            //    "+" + phone,
            //    text)));
            wc.DownloadString($"http://smsc.ru/sys/send.php?charset=utf-8&login=sendika&psw=DieF3Umie5aichox&phones=+{phone}&mes={text}");
                    
        }
    }
}