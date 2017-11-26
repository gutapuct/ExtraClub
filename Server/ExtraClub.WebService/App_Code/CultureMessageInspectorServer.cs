using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel.Dispatcher;
using System.Threading;
using System.Globalization;


public class CultureMessageInspectorServer : IDispatchMessageInspector
{
    private const string HeaderKey = "culture";

    public object AfterReceiveRequest(ref System.ServiceModel.Channels.Message request, System.ServiceModel.IClientChannel channel, System.ServiceModel.InstanceContext instanceContext)
    {
        int headerIndex = request.Headers.FindHeader(HeaderKey, string.Empty);
        if (headerIndex != -1)
        {
            var cp = request.Headers.GetHeader<string>(headerIndex);
            CultureInfo ci;
            if (cp == "en-US")
            {
                ci = System.Globalization.CultureInfo.GetCultureInfo("en-US").Clone() as CultureInfo;
                ci.NumberFormat.CurrencySymbol = "€";
                ci.NumberFormat.CurrencyPositivePattern = 2;
                ci.NumberFormat.CurrencyNegativePattern = 2;
            }
            else if (cp == "ru-KZ")
            {
                ci = System.Globalization.CultureInfo.GetCultureInfo("ru-RU").Clone() as CultureInfo;
                ci.NumberFormat.CurrencySymbol = "тг.";
                ci.NumberFormat.CurrencyPositivePattern = 3;
                ci.NumberFormat.CurrencyNegativePattern = 3;
            }
            else if (cp == "ru-UZ")
            {
                ci = System.Globalization.CultureInfo.GetCultureInfo("ru-RU").Clone() as CultureInfo;
                ci.NumberFormat.CurrencySymbol = "сум.";
                ci.NumberFormat.CurrencyPositivePattern = 3;
                ci.NumberFormat.CurrencyNegativePattern = 3;
            }
            else
            {
                ci = new CultureInfo(cp);
            }
            Thread.CurrentThread.CurrentUICulture = ci;
            Thread.CurrentThread.CurrentCulture = ci;
        }
        return null;
    }

    public void BeforeSendReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
    {
    }
}