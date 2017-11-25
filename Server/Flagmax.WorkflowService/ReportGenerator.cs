using ClosedXML.Excel;
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml.Linq;
using TonusClub.Entities;
using TonusClub.ServerCore;
using TonusClub.ServiceModel;

namespace Flagmax.WorkflowService
{
    public class ReportGenerator
    {
        public CancelThreadInfo CancelThreadInfo { get; private set; }
        TonusEntities Context = new TonusEntities();
        DateTime Date;

        public ReportGenerator(CancelThreadInfo cti, DateTime date)
        {
            CancelThreadInfo = cti;
            Date = date;
        }

        public Thread RunAsync()
        {
            var res = new Thread(new ThreadStart(Run));
            res.Start();
            return res;
        }

        public void Run()
        {
            var dayOfMonth = Date.Day;
            var dayOfWeek = (int)Date.DayOfWeek;
            if(dayOfWeek == 0)
            {
                dayOfWeek = 7;
            }
            var matchedReports = Context.ReportRecurrencies
                .Where(i => !Context.RecurrentSentInfos.Any(j => j.RuleId == i.Id && j.SentDate == DateTime.Today))
                .Where(i => (i.Recurrency == 0 && i.PeriodDay == dayOfMonth) || (i.Recurrency == 1 && i.PeriodDay == dayOfWeek) || i.Recurrency == 2).ToArray();
            var userIds = matchedReports.Select(i => i.UserId).Distinct().ToArray();
            var users = Context.Users.Where(i => userIds.Contains(i.UserId)).Select(i => new { i.UserName, i.UserId, i.Email, i.CompanyId, i.FullName }).ToDictionary(i => i.UserId, i => i);
            var reports = matchedReports.Where(i => !String.IsNullOrWhiteSpace(users[i.UserId].Email) && ValidateEmail(users[i.UserId].Email)).GroupBy(i => i.UserId).ToArray();


            foreach(var userSet in reports)
            {
                UserManagement.ManualUserId = userSet.Key;
                var letter = new StringBuilder();
                letter.AppendFormat("Здравствуйте, {0}!<br/>\n", users[userSet.Key].FullName);
                letter.AppendLine("<br/>Сегодняшний набор отчетов в приложенном файле содержит:");
                var reportInfos = ReportsCore.GetUserReportsList().Where(i => i.Type == TonusClub.ServiceModel.Reports.ReportType.Stored || i.Type == TonusClub.ServiceModel.Reports.ReportType.Code)
                    .GroupBy(i => i.Key)
                    .ToDictionary(i => i.Key, i => i.FirstOrDefault());
                var wb = new XLWorkbook();
                int num = 1;
                foreach(var report in userSet)
                {
                    if(!reportInfos.ContainsKey(report.ReportKey))
                    {
                        continue;
                    }
                    var reportInfo = reportInfos[report.ReportKey];
                    var tableName = reportInfo.Name;

                    letter.AppendFormat("<b>{0}. {1}</b><br/>\n", num, tableName);
                    try
                    {
                        var parameters = new Dictionary<string, object>();
                        var serParameters = ReportRecurrency.DeserializeParameters(report.Parameters);
                        if(serParameters.Period.HasValue)
                        {
                            //{0, "За вчера"},
                            //{1, "С начала текущего месяца"},
                            //{2, "За прошлый месяц"},
                            //{3, "За последние 30 дней"},
                            //{4, "С начала текущей недели"},
                            //{5, "За прошлую неделю"},
                            //{6, "За последние 7 дней"}
                            DateTime start = Date.AddDays(-1), end = Date.AddDays(-1);
                            switch(serParameters.Period.Value)
                            {
                                case 1:
                                    start = new DateTime(Date.Year, Date.Month, 1);
                                    end = Date;
                                    break;
                                case 2:
                                    start = new DateTime(Date.Year, Date.Month, 1).AddMonths(-1);
                                    end = new DateTime(Date.Year, Date.Month, 1).AddDays(-1);
                                    break;
                                case 3:
                                    start = Date.AddDays(-30);
                                    break;
                                case 4:
                                    start = GetFirstDateOfWeek(Date);
                                    end = Date;
                                    break;
                                case 5:
                                    start = GetFirstDateOfWeek(Date.AddDays(-7));
                                    end = start.AddDays(6);
                                    break;
                                case 6:
                                    start = Date.AddDays(-7);
                                    break;
                            }

                            parameters.Add("start", start);
                            parameters.Add("end", end);
                        }
                        foreach(var par in serParameters.Parameters)
                        {
                            parameters.Add(par.Item1, par.Item2);
                            var par2Text = par.Item2 == null ? "Не указан" : par.Item2.ToString();
                            if(par2Text == "True") par2Text = "Да";
                            if(par2Text == "False") par2Text = "Нет";
                            if(par2Text.Length == Guid.Empty.ToString().Length) continue;
                            letter.AppendFormat("<b>{0}</b>: {1}<br/>\n", reportInfo.Parameters.Where(i => i.InternalName == par.Item1).Select(i => i.DisplayName).FirstOrDefault() ?? par.Item1,
                                par2Text);
                        }
                        var table = ReportsCore.GenerateReportUncompressed(report.ReportKey, parameters);
                        table.TableName = num.ToString() + " " + tableName.Substring(0, Math.Min(tableName.Length, 28));
                        for(var i = table.Columns.Count - 1; i >= 0; i--)
                        {
                            if(table.Columns[i].ColumnName.StartsWith("_"))
                            {
                                table.Columns.RemoveAt(i);
                            }
                        }
                        wb.Worksheets.Add(table);
                        num++;
                    }
                    catch(Exception ex)
                    {
                        letter.AppendFormat("<i>При формировании отчета произошла ошибка, отчет не был включен в письмо:<br/>{0}</i><br/></br>\n", ex.Message);
                    }

                }
                var ms = new MemoryStream();
                wb.SaveAs(ms);
                ms.Position = 0;

                ms = ApplyOpenXmlFix(ms);
                ms.Position = 0;

                try
                {
                    NotificationCore.SendMessage(users[userSet.Key].Email, "Автоматическая отправка отчетов", letter.ToString(), ms, "Отчеты.xlsx");

                    userSet.ForEach(i =>
                        {
                            i.LastSentDate = DateTime.Today;
                            Context.RecurrentSentInfos.AddObject(new RecurrentSentInfo
                            {
                                Id = Guid.NewGuid(),
                                RuleId = i.Id,
                                SentDate = DateTime.Today
                            });
                        });
                    Context.SaveChanges();
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
            }
        }

        private MemoryStream ApplyOpenXmlFix(MemoryStream input)
        {
            const string RELS_FILE = @"xl/_rels/workbook.xml.rels";
            const string RELATIONSHIP_ELEMENT = "Relationship";
            const string CONTENT_TYPE_FILE = @"[Content_Types].xml";
            const string XL_WORKBOOK_XML = "/xl/workbook.xml";
            const string TARGET_ATTRIBUTE = "Target";
            const string SUPERFLUOUS_PATH = "/xl/";
            const string OVERRIDE_ELEMENT = "Override";
            const string PARTNAME_ATTRIBUTE = "PartName";
            const string CONTENTTYPE_ATTRIBUTE = "ContentType";
            const string CONTENTTYPE_VALUE = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml";

            XNamespace contentTypesNamespace = "http://schemas.openxmlformats.org/package/2006/content-types";
            XNamespace relsNamespace = "http://schemas.openxmlformats.org/package/2006/relationships";
            XDocument xlDocument;
            MemoryStream memWriter;

            try
            {
                input.Seek(0, SeekOrigin.Begin);
                ZipFile zip = ZipFile.Read(input);

                //First we fix the workbook relations file
                var workbookRelations = zip.Entries.Where(e => e.FileName == RELS_FILE).Single();
                xlDocument = XDocument.Load(workbookRelations.OpenReader());

                //Remove the /xl/ relative path from all target attributes
                foreach(var relationship in xlDocument.Root.Elements(relsNamespace + RELATIONSHIP_ELEMENT))
                {
                    var target = relationship.Attribute(TARGET_ATTRIBUTE);

                    if(target != null && target.Value.StartsWith(SUPERFLUOUS_PATH))
                    {
                        target.Value = target.Value.Substring(SUPERFLUOUS_PATH.Length);
                    }
                }

                //Replace the content in the source zip file
                memWriter = new MemoryStream();
                xlDocument.Save(memWriter, SaveOptions.DisableFormatting);
                memWriter.Seek(0, SeekOrigin.Begin);
                zip.UpdateEntry(RELS_FILE, memWriter);

                //Now we fix the content types XML file
                var contentTypeEntry = zip.Entries.Where(e => e.FileName == CONTENT_TYPE_FILE).Single();
                xlDocument = XDocument.Load(contentTypeEntry.OpenReader());

                if(!xlDocument.Root.Elements().Any(e =>
                    e.Name == contentTypesNamespace + OVERRIDE_ELEMENT &&
                    e.Attribute(PARTNAME_ATTRIBUTE) != null &&
                    e.Attribute(PARTNAME_ATTRIBUTE).Value == XL_WORKBOOK_XML))
                {
                    //Add in the missing element
                    var overrideElement = new XElement(
                        contentTypesNamespace + OVERRIDE_ELEMENT,
                        new XAttribute(PARTNAME_ATTRIBUTE, XL_WORKBOOK_XML),
                        new XAttribute(CONTENTTYPE_ATTRIBUTE, CONTENTTYPE_VALUE));

                    xlDocument.Root.Add(overrideElement);

                    //Replace the content
                    memWriter = new MemoryStream();
                    xlDocument.Save(memWriter, SaveOptions.DisableFormatting);
                    memWriter.Seek(0, SeekOrigin.Begin);
                    zip.UpdateEntry(CONTENT_TYPE_FILE, memWriter);
                }

                var output = new MemoryStream();

                //Save file
                zip.Save(output);

                return output;
            }
            catch
            {
                //Just in case it fails, return the original document
                return input;
            }
        }
        public static DateTime GetFirstDateOfWeek(DateTime dayInWeek)
        {
            DateTime firstDayInWeek = dayInWeek.Date;
            while(firstDayInWeek.DayOfWeek != DayOfWeek.Monday)
            {
                firstDayInWeek = firstDayInWeek.AddDays(-1);
            }
            return firstDayInWeek;
        }

        public static bool ValidateEmail(string email)
        {
            string pattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|"
              + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)"
              + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";

            var regex = new Regex(pattern, RegexOptions.IgnoreCase);
            return regex.IsMatch(email);
        }
    }
}
