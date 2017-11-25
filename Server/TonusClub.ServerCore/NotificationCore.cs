using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TonusClub.Entities;
using TonusClub.ServiceModel;
using System.Net.Mail;
using System.Configuration;
using System.Net;
using System.IO;
using System.Net.Mime;

namespace TonusClub.ServerCore
{
    public static class NotificationCore
    {
        public static void SendMail(Division div, Exception exc, string receps, string log = null)
        {
            try
            {
                if (string.IsNullOrEmpty(receps)) return;
                var message = new MailMessage();
                receps.Split(new char[] { '\n', ',', ';' }, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(i => message.To.Add(i.Replace("\r", "").Replace(" ", "")));
                message.From = new System.Net.Mail.MailAddress(ConfigurationManager.AppSettings.Get("SmtpLogin"));
                if (exc == null)
                {
                    message.Subject = "Успешная синхронизация " + div.Name;
                    message.Body = Localization.Resources.SuccessfulSync;
                }
                else
                {
                    message.Subject = Localization.Resources.SyncErr;
                    message.Body = String.Format("При синхронизации произошла ошибка.\n\n{0}\n{1}\n{2}\n\nОтчет:\n{3}", exc.GetType().ToString(), exc.Message, exc.StackTrace, log);
                }
                var smtp = new SmtpClient
                {
                    Host = ConfigurationManager.AppSettings.Get("SmtpHost"),
                    Port = Int32.Parse(ConfigurationManager.AppSettings.Get("SmtpPort"))
                };
                smtp.EnableSsl = ConfigurationManager.AppSettings.Get("UseSSL") == "1";
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                smtp.Credentials = new NetworkCredential(ConfigurationManager.AppSettings.Get("SmtpLogin"), ConfigurationManager.AppSettings.Get("SmtpPassword"));
                smtp.Send(message);
            }
            catch
            {
                //TODO: Add Log
            }
        }

        public static void SendLocalMail(string log, string subject = "Отчет о синхронизации")
        {
            try
            {
                var loc = new TonusEntities().LocalSettings.FirstOrDefault();
                if (String.IsNullOrEmpty(loc.NotifyAdresses)) return;
                var message = new MailMessage();
                loc.NotifyAdresses.Split(new char[] { '\n', ',', ';' }, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(i => message.To.Add(i.Replace("\r", "").Replace(" ", "")));
                message.From = new System.Net.Mail.MailAddress(ConfigurationManager.AppSettings.Get("SmtpLogin"));
                message.Subject = subject;
                message.Body = log;

                var smtp = new SmtpClient
                {
                    Host = ConfigurationManager.AppSettings.Get("SmtpHost"),
                    Port = Int32.Parse(ConfigurationManager.AppSettings.Get("SmtpPort"))
                };
                smtp.EnableSsl = ConfigurationManager.AppSettings.Get("UseSSL") == "1";
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                smtp.Credentials = new NetworkCredential(ConfigurationManager.AppSettings.Get("SmtpLogin"), ConfigurationManager.AppSettings.Get("SmtpPassword"));
                smtp.Send(message);
            }
            catch
            {
                //TODO: Add Log
            }
        }

        public static void SendMessage(string destination, string subject, string body)
        {
            SendMessage(destination, subject, body, null, null);
        }

        public static void SendMessage(string destination, string subject, string body, Stream attach, string attachName)
        {
            return;
            var message = new MailMessage();
            message.To.Add(destination);
            message.From = new MailAddress(ConfigurationManager.AppSettings.Get("SmtpFrom"), ConfigurationManager.AppSettings.Get("SmtpFromName"));
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = body.Contains("<");
            var smtp = new SmtpClient
            {
                Host = ConfigurationManager.AppSettings.Get("SmtpHost"),
                Port = Int32.Parse(ConfigurationManager.AppSettings.Get("SmtpPort"))
            };
            smtp.EnableSsl = ConfigurationManager.AppSettings.Get("UseSSL") == "1";
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            if(attach != null)
            {
                message.Attachments.Add(AttachmentHelper.CreateAttachment(attach, attachName, TransferEncoding.Base64));
            }

            smtp.Credentials = new NetworkCredential(ConfigurationManager.AppSettings.Get("SmtpLogin"), ConfigurationManager.AppSettings.Get("SmtpPassword"));
            smtp.Send(message);
        }

        public class AttachmentHelper
        {
            public static Attachment CreateAttachment(Stream attachmentStream, string displayName, TransferEncoding transferEncoding)
            {
                Attachment attachment = new Attachment(attachmentStream, displayName);
                attachment.TransferEncoding = transferEncoding;

                string tranferEncodingMarker = String.Empty;
                string encodingMarker = String.Empty;
                int maxChunkLength = 0;

                switch(transferEncoding)
                {
                    case TransferEncoding.Base64:
                        tranferEncodingMarker = "B";
                        encodingMarker = "UTF-8";
                        maxChunkLength = 30;
                        break;
                    case TransferEncoding.QuotedPrintable:
                        tranferEncodingMarker = "Q";
                        encodingMarker = "ISO-8859-1";
                        maxChunkLength = 76;
                        break;
                    default:
                        throw (new ArgumentException(String.Format("The specified TransferEncoding is not supported: {0}", transferEncoding, "transferEncoding")));
                }

                attachment.NameEncoding = Encoding.GetEncoding(encodingMarker);

                string encodingtoken = String.Format("=?{0}?{1}?", encodingMarker, tranferEncodingMarker);
                string softbreak = "?=";
                string encodedAttachmentName = encodingtoken;

                if(attachment.TransferEncoding == TransferEncoding.QuotedPrintable)
                    encodedAttachmentName = System.Uri.EscapeDataString(displayName).Replace("+", " ").Replace("%", " ").Replace("=", " ");
                else
                    encodedAttachmentName = Convert.ToBase64String(Encoding.UTF8.GetBytes(displayName));

                encodedAttachmentName = SplitEncodedAttachmentName(encodingtoken, softbreak, maxChunkLength, encodedAttachmentName);
                attachment.Name = encodedAttachmentName;

                return attachment;
            }

            private static string SplitEncodedAttachmentName(string encodingtoken, string softbreak, int maxChunkLength, string encoded)
            {
                int splitLength = maxChunkLength - encodingtoken.Length - (softbreak.Length * 2);
                var parts = SplitByLength(encoded, splitLength);

                string encodedAttachmentName = encodingtoken;

                foreach(var part in parts)
                    encodedAttachmentName += part + softbreak + encodingtoken;

                encodedAttachmentName = encodedAttachmentName.Remove(encodedAttachmentName.Length - encodingtoken.Length, encodingtoken.Length);
                return encodedAttachmentName;
            }

            private static IEnumerable<string> SplitByLength(string stringToSplit, int length)
            {
                while(stringToSplit.Length > length)
                {
                    yield return stringToSplit.Substring(0, length);
                    stringToSplit = stringToSplit.Substring(length);
                }

                if(stringToSplit.Length > 0) yield return stringToSplit;
            }
        }

    }
}
