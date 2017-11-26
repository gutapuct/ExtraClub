using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using ExtraClub.Infrastructure;

namespace ExtraClub.OrganizerModule.Business
{
    public static class FtmUtility
    {
        public static void GetCommentsAsync(int taskNumber, Action<IEnumerable<CommentInfo>> onComplete)
        {
            try
            {
                using(var wc = new WebClient { Encoding = Encoding.UTF8 })
                {
                    wc.DownloadStringCompleted += delegate(object sender, DownloadStringCompletedEventArgs evs)
                    {
                        if(!evs.Cancelled && evs.Error == null)
                        {
                            onComplete(JsonConvert.DeserializeObject<CommentInfo[]>(evs.Result));
                        }
                    };
                    wc.DownloadStringAsync(new Uri("http://ftm.sendika.ru/External/GetCommentsJson/" + taskNumber));
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        internal static void PostComment(int taskNumber, string text)
        {
            using(var wc = new WebClient { Encoding = Encoding.UTF8 })
            {
                try
                {
                    System.Collections.Specialized.NameValueCollection reqparm = new System.Collections.Specialized.NameValueCollection();
                    reqparm.Add("id", taskNumber.ToString());
                    reqparm.Add("text", text);
                    wc.UploadValues("http://ftm.sendika.ru/External/PostComment/", "POST", reqparm);
                }
                catch (Exception ex)
                {
                    throw new Exception("Невозможно отправить комментарий!", ex);
                }
            }
        }

        internal static void PostComplaint(int taskNumber, string text)
        {
            using (var wc = new WebClient { Encoding = Encoding.UTF8 })
            {
                try
                {
                    System.Collections.Specialized.NameValueCollection reqparm = new System.Collections.Specialized.NameValueCollection();
                    reqparm.Add("id", taskNumber.ToString());
                    reqparm.Add("text", text);
                    wc.UploadValues("http://ftm.sendika.ru/External/PostComplaint/", "POST", reqparm);
                }
                catch (Exception ex)
                {
                    throw new Exception("Невозможно отправить жалобу!", ex);
                }
            }
        }
    }
}
