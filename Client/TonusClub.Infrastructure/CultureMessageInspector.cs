using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using System.Threading;
using System.Globalization;

namespace TonusClub.Infrastructure
{
    public class CultureMessageInspector : IClientMessageInspector
    {
        private const string HeaderKey = "culture";

        public object BeforeSendRequest
              (ref System.ServiceModel.Channels.Message request,
              System.ServiceModel.IClientChannel channel)
        {
            if (AppSettingsManager.GetSetting("Language") == "0")
            {
                request.Headers.Add(MessageHeader.CreateHeader
                    (HeaderKey, string.Empty, "ru-RU"));
            }
            else if(AppSettingsManager.GetSetting("Language") == "1")
            {
                request.Headers.Add(MessageHeader.CreateHeader
                    (HeaderKey, string.Empty, "en-US"));
            }
            else if(AppSettingsManager.GetSetting("Language") == "3")
            {
                request.Headers.Add(MessageHeader.CreateHeader
                    (HeaderKey, string.Empty, "ru-UZ"));
            }
            else
            {
                request.Headers.Add(MessageHeader.CreateHeader
                    (HeaderKey, string.Empty, "ru-KZ"));
            }
            return null;
        }

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            
        }
    }

}
