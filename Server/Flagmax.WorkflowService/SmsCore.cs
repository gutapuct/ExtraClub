using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.EntityClient;
using System.Data.Objects;
using System.Data.Objects.SqlClient;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using TonusClub.Entities;
using TonusClub.ServerCore;
using TonusClub.ServiceModel;

namespace Flagmax.WorkflowService
{
    public class SmsCore : IDisposable
    {
        public static bool ValidatePhone(string phone)
        {
            if (String.IsNullOrWhiteSpace(phone))
            {
                return false;
            }
            phone = FormatPhone(phone);
            if (!phone.StartsWith("79") && !phone.StartsWith("89"))
            {
                return false;
            }
            if (phone.Length != 11)
            {
                return false;
            }
            if (phone.Contains("000000") || phone.Contains("111111") || phone.Contains("222222") ||
                phone.Contains("333333") || phone.Contains("444444") || phone.Contains("555555") ||
                phone.Contains("777777") || phone.Contains("888888") || phone.Contains("999999") ||
                phone.Contains("666666"))
            {
                return false;
            }
            return true;
        }

        TonusEntities context = new TonusEntities() { CommandTimeout = 600 };

        Dictionary<Guid, Tuple<short, string>> _divisionIds;

        public SmsCore()
        {
            _divisionIds = new Dictionary<Guid, Tuple<short, string>>();
            using (var conn = new SqlConnection(((EntityConnection)context.Connection).StoreConnection.ConnectionString))
            {
                conn.Open();
                using (var rdr = new SqlCommand(
@"select d.Id, isnull(UtcCorr, 0), isnull(ltrim(rtrim(replace(mv.StringValue, 'тел.', ''))),'')
from Divisions d
inner join companies c on c.companyid=d.companyid
inner join crm..Clubs cl on cl.DivisionId = d.Id
left join crm..MetaValues mv on mv.EntityId = cl.Id and mv.MetaFieldId = '293DC2D5-8EED-4D76-8299-84CE147AFAD7'
where d.id in (
	select DivisionId
	from syncmetadata.dbo.MetaCompanies
	where SendSms = 1 and SmsTreatments=1
)", conn)
                { CommandTimeout = 300 }.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        _divisionIds.Add(rdr.GetGuid(0), Tuple.Create(rdr.GetInt16(1), rdr.GetString(2)));
                    }
                }
                conn.Close();
            }
        }

        public void DoSmsGen(TonusClub.ServerCore.CancelThreadInfo cancelInfo)
        {
            var endDate = DateTime.Today.AddDays(2);
            var dIds = _divisionIds.Keys.ToArray();
            var bulk = context.TreatmentEvents
                .Where(i => dIds.Contains(i.DivisionId))
                .Where(i => i.VisitDate >= DateTime.Today && i.VisitDate < endDate)
                .Where(i => SqlFunctions.DateDiff("day", EntityFunctions.TruncateTime(i.CreatedOn), EntityFunctions.TruncateTime(i.VisitDate)) > 1)
                .Where(i => i.VisitStatus == 0)
                .Where(i => i.Customer.SmsList && i.Customer.Phone2.Length >= 10)
                .Where(i => !context.SmsMessages.Any(j => j.CustomerId == i.CustomerId &&
                    SqlFunctions.DateAdd("day", 1, EntityFunctions.TruncateTime(j.ToSendFrom)) == EntityFunctions.TruncateTime(i.VisitDate)))
                .GroupBy(i => new { i.Customer.Id, i.Customer.Phone2, i.Customer.FirstName, i.Customer.MiddleName })
                .Select(i => new
                {
                    CustomerId = i.Key.Id,
                    Phone = i.Key.Phone2,
                    Name = (i.Key.FirstName ?? "") + ((" " + i.Key.MiddleName) ?? ""),
                    Date = i.Min(j => j.VisitDate),
                    DivisionId = i.FirstOrDefault().DivisionId
                })
                .ToArray();
            int n = 0;
            foreach (var item in bulk)
            {
                if (cancelInfo.Cancel)
                {
                    return;
                }
                var phone = FormatPhone(item.Phone);
                if (!ValidatePhone(phone))
                {
                    continue;
                }
                var date = item.Date.AddHours(_divisionIds[item.DivisionId].Item1);
                var phoneClub = (!String.IsNullOrWhiteSpace(_divisionIds[item.DivisionId].Item2)) ? " Тел: " + _divisionIds[item.DivisionId].Item2 : "";
                context.SmsMessages.AddObject(
                    new SmsMessage
                    {
                        CustomerId = item.CustomerId,
                        Id = Guid.NewGuid(),
                        Phone = phone,
                        ToSendFrom = item.Date.AddDays(-1).AddHours(2),
                        Text = String.Format("{0}, ждем Вас завтра в {1:HH:mm}.{2}", item.Name, date, phoneClub),
                    });
                if ((++n) % 50 == 0)
                {
                    context.SaveChanges();
                    n = 0;
                }
            }
            context.SaveChanges();
        }

        internal static string FormatPhone(string phone)
        {
            phone = (phone ?? "").Replace(" ", "").Replace("-", "").Replace("+", "").Replace("(", "").Replace(")", "");
            if (phone.Length > 0 && phone.StartsWith("8"))
            {
                phone = "7" + phone.Substring(1);
            }
            return phone;
        }

        public void Dispose()
        {
            context.Dispose();
        }

        public void SendPending(TonusClub.ServerCore.CancelThreadInfo cancelInfo)
        {
            var pendingDay = context.SmsMessages.Where(i => !i.SentOn.HasValue && i.ToSendFrom <= DateTime.Now).ToArray();
            if (pendingDay.Length > 0)
            {
                Logger.Log("Начало отправки СМС");
            }
            foreach (var item in pendingDay)
            {
                if (cancelInfo.Cancel)
                {
                    return;
                }
                try
                {
                    if (item.ToSendFrom.Date == DateTime.Today)
                    {
                        item.Report = SendOneSms(item.Phone, item.Text);
                    }
                    else
                    {
                        item.Report = String.Format("Не отправлено, т.к. просрочено");
                    }
                    item.SentOn = DateTime.Now;
                }
                catch (Exception ex)
                {
                    Log.WriteLine("Ошибка при отправке Смс " + item.Id.ToString());
                    Log.WriteLine(ex.Message);
                    Log.WriteLine(ex.StackTrace);
                    item.SentOn = DateTime.Now;
                    item.Report = ex.Message;
                }
                context.SaveChanges();
            }
        }

        public void CheckPending(TonusClub.ServerCore.CancelThreadInfo cancelInfo)
        {
            var pending = context.SmsMessages.Where(i => !i.SentOn.HasValue && !i.SkipCheck).Select(i => new { i.Id, i.CustomerId, ForDate = SqlFunctions.DateAdd("day", 1, EntityFunctions.TruncateTime(i.ToSendFrom)) }).ToArray();
            var cIds = pending.Select(i => i.CustomerId).ToArray();
            var dates = pending.Select(i => i.ForDate).ToArray();

            if (cancelInfo.Cancel)
            {
                return;
            }

            var treatments = context.TreatmentEvents
                .Where(i => cIds.Contains(i.CustomerId) && dates.Contains(EntityFunctions.TruncateTime(i.VisitDate).Value) && i.VisitStatus == 0)
                .Select(i => new { i.CustomerId, Date = EntityFunctions.TruncateTime(i.VisitDate) }).ToArray();

            if (cancelInfo.Cancel)
            {
                return;
            }

            var toCancel = pending.Where(i => !treatments.Any(j => j.CustomerId == i.CustomerId && j.Date == i.ForDate)).Select(i => i.Id).ToArray();
            foreach (var sms in context.SmsMessages.Where(i => toCancel.Contains(i.Id)).ToArray())
            {
                if (cancelInfo.Cancel)
                {
                    return;
                }

                if (!sms.SentOn.HasValue)
                {
                    sms.SentOn = DateTime.Now;
                    sms.Report = "Отмена занятия";
                }
            }
            context.SaveChanges();

        }

        private string SendOneSms(string phone, string text)
        {
            var wc = new WebClient();
            //var str = wc.DownloadString(String.Format(
            //    "http://gateway.api.sc/get/send_xml.php?user={0}&pwd={1}&sadr={2}&dadr={3}&text={4}",
            //    ConfigurationManager.AppSettings.Get("SmsLoginNew"),
            //    ConfigurationManager.AppSettings.Get("SmsPasswordNew"),
            //    ConfigurationManager.AppSettings.Get("SmsSenderName"),
            //    "+" + phone,
            //    text));

            var _user = ConfigurationManager.AppSettings.Get("SmsLoginNew");
            var _password = ConfigurationManager.AppSettings.Get("SmsPasswordNew");
            var _phone = "+" + phone;

            var str = wc.DownloadString($"http://smsc.ru/sys/send.php?charset=utf-8&login={_user}&psw={_password}&phones={_phone}&mes={text}");
            return str;
        }
    }
}
