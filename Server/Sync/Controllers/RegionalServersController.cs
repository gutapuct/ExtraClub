using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sync.Models;
using TonusClub.Entities;
using System.Web.Services;
using System.Collections;
using System.IO;

namespace Sync.Controllers
{
    [Authorize]
    public class RegionalServersController : Controller
    {
        public ActionResult Index(bool showAll = true, bool isArchive = false, int sort = 0)
        {
            ViewBag.CanCommit = AccountController.IsWebAdmin(User.Identity.Name);

            ViewBag.ShowAll = showAll;
            ViewBag.isArchive = isArchive;
            ViewBag.Sort = sort;
            using(var main = new TonusEntities())
            using(var context = new SyncMetadataEntities())
            {
                var res = context.MetaCompanies
                    .ToList()
                    .Where(i => (showAll || (i.LicenceTillUncommited.HasValue || ((i.LicenseTill ?? DateTime.MaxValue) - DateTime.Now).TotalDays < 30) || !i.IsLicenseAvailable))
                    .ToList().Select(i => new RegionalServerView
                {
                    CompanyName = main.Companies.Single(j => j.CompanyId == i.CompanyId).CompanyName,
                    DivisionName = main.Divisions.Single(j => j.Id == i.DivisionId).Name,
                    ContactPerson = i.ContactPerson,
                    LastSync = i.LastSyncDate,
                    LastKeyReceived = i.LastKeyReceived,
                    MaxSyncPeriod = i.MaxSyncPeriod ?? 0,
                    Id = i.Id,
                    LicenseTill = i.LicenseTill,
                    LicenseTillUncommited = i.LicenceTillUncommited,
                    IsLicenseAvailable = i.IsLicenseAvailable,
                    Versions = (i.WsVersion ?? "-") + "/" + (i.WfVersion ?? "-") + "/" + (i.DbVersion ?? "-") + "/" + (i.UpdVersion ?? "-"),
                    SendSms = i.SendSms,
                    SmsUntil = i.SmsUntil,
                    SmsMarketing = i.SmsMarketing,
                    SmsBirthday = i.SmsBirthday,
                    SmsTreatments = i.SmsTreatments
                }
                ).ToList();
                if(sort == 0) res = res.OrderBy(i => i.CompanyName).ToList();
                if(sort == 1) res = res.OrderByDescending(i => i.LicenseTill).ToList();
                if(sort == 2) res = res.OrderByDescending(i => i.LastSync).ToList();
                if(!isArchive)
                {
                    res = res.Where(k => k.LicenseTill > DateTime.Now.AddMonths(-1)).ToList();
                    //var ctl = new UpdaterController();
                    //var v = String.Format("{0}/{1}/{2}/{3}",
                    //    ctl.GetVersionNumber("wsupdates"),
                    //    ctl.GetVersionNumber("wfupdates"),
                    //    ((ContentResult)ctl.GetCurrentDBVersion()).Content,
                    //    ctl.GetVersionNumber("updupdates"));

                    //res = res.Where(i => i.Versions != v).ToList();
                }
                return View(res);
            }
        }

        public ActionResult Details(Guid id, string error = "")
        {
            using(var main = new TonusEntities())
            using(var context = new SyncMetadataEntities())
            {
                if(!String.IsNullOrEmpty(error))
                {
                    this.ModelState.AddModelError(String.Empty, error);
                }
                var i = context.MetaCompanies.FirstOrDefault(j => j.Id == id);
                if(i == null) return RedirectToActionPermanent("Index");
                return View(new RegionalServerView
                {
                    CompanyName = main.Companies.Single(j => j.CompanyId == i.CompanyId).CompanyName,
                    DivisionName = main.Divisions.Single(j => j.Id == i.DivisionId).Name,
                    LastSync = i.LastSyncDate,
                    Id = i.Id,
                    HWKey = i.MachineKey,
                    EmailFailure = i.EmailFailure,
                    EmailSuccess = i.EmailSuccess,
                    IsLicenseAvailable = i.IsLicenseAvailable,
                    LicenseTill = i.LicenseTill,
                    MaxSyncPeriod = i.MaxSyncPeriod ?? 0,
                    ContactPerson = i.ContactPerson,
                    FirstSync = i.FirstSync,
                    LastKeyReceived = i.LastKeyReceived,
                    SendSms = i.SendSms,
                    SmsUntil = i.SmsUntil,
                    SmsMarketing = i.SmsMarketing,
                    SmsBirthday = i.SmsBirthday,
                    SmsTreatments = i.SmsTreatments
                });
            }
        }

        public ActionResult Create()
        {
            using(var meta = new SyncMetadataEntities())
            using(var context = new TonusEntities())
            {
                ViewBag.Divisions = context.Divisions.OrderBy(i => i.Company.CompanyName).ThenBy(i => i.Name).ToList().Where(i => !meta.MetaCompanies.Any(k => k.DivisionId == i.Id)).ToList().Select(i => new SelectListItem { Value = i.Id.ToString(), Text = i.Company.CompanyName + ", " + i.Name }).ToList();
                return View();
            }
        }

        public ActionResult Download(string id)
        {
            try
            {
                return File(new Service().GetLicenceKey(id), "application/license", "certkey.tlk");
            }
            catch(Exception ex)
            {
                using(var context = new SyncMetadataEntities())
                {
                    var i = context.MetaCompanies.FirstOrDefault(j => j.MachineKey == id);
                    return RedirectToAction("Details", new { id = i.Id, error = ex.Message });
                }
            }
        }

        [HttpPost]
        public ActionResult Create(RegionalServerView model)
        {
            try
            {
                using(var meta = new SyncMetadataEntities())
                using(var context = new TonusEntities())
                {
                    var div = context.Divisions.Single(i => i.Id == model.DivisionId);
                    var comp = new MetaCompany
                    {
                        Id = Guid.NewGuid(),
                        CompanyId = div.CompanyId,
                        DivisionId = div.Id,
                        MachineKey = model.HWKey,
                        EmailFailure = model.EmailFailure ?? "",
                        EmailSuccess = model.EmailSuccess ?? "",
                        IsLicenseAvailable = model.IsLicenseAvailable,
                        LicenseTill = model.LicenseTill,
                        MaxSyncPeriod = model.MaxSyncPeriod,
                        ContactPerson = model.ContactPerson,
                        SendSms = false,
                        SmsMarketing = model.SmsMarketing,
                        SmsBirthday = model.SmsBirthday,
                        SmsTreatments = model.SmsTreatments
                    };
                    meta.MetaCompanies.AddObject(comp);
                    meta.SaveChanges();
                    TrackChanges("Новый РС", comp.Id, null, null);
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult CommitLicense(Guid id)
        {
            using(var context = new SyncMetadataEntities())
            {
                var company = context.MetaCompanies.SingleOrDefault(i => i.Id == id);
                if(company != null)
                {
                    TrackChanges("Подтверждение срока лицензии", company.Id, company.LicenseTill, company.LicenceTillUncommited);

                    company.LicenseTill = company.LicenceTillUncommited;
                    company.LicenceTillUncommited = null;
                    context.SaveChanges();
                }
                return RedirectToAction("Index");
            }
        }

        public ActionResult RejectLicense(Guid id)
        {
            using(var context = new SyncMetadataEntities())
            {
                var company = context.MetaCompanies.SingleOrDefault(i => i.Id == id);
                if(company != null)
                {
                    company.LicenceTillUncommited = null;
                    context.SaveChanges();
                }
                return RedirectToAction("Index");
            }
        }

        public ActionResult Edit(Guid id)
        {
            using(var main = new TonusEntities())
            using(var context = new SyncMetadataEntities())
            {
                var i = context.MetaCompanies.FirstOrDefault(j => j.Id == id);
                if(i == null) return RedirectToActionPermanent("Index");
                return View(new RegionalServerView
                {
                    CompanyName = main.Companies.Single(j => j.CompanyId == i.CompanyId).CompanyName,
                    DivisionName = main.Divisions.Single(j => j.Id == i.DivisionId).Name,
                    LastSync = i.LastSyncDate,
                    Id = i.Id,
                    HWKey = i.MachineKey,
                    EmailFailure = i.EmailFailure ?? "",
                    EmailSuccess = i.EmailSuccess ?? "",
                    IsLicenseAvailable = i.IsLicenseAvailable,
                    LicenseTill = i.LicenseTill,
                    MaxSyncPeriod = i.MaxSyncPeriod ?? 0,
                    LicenseTillUncommited = i.LicenceTillUncommited,
                    ContactPerson = i.ContactPerson,
                    LastUpdated = i.LastUpdated,
                    SendSms = i.SendSms,
                    SmsUntil = i.SmsUntil,
                    SmsMarketing = i.SmsMarketing,
                    SmsBirthday = i.SmsBirthday,
                    SmsTreatments = i.SmsTreatments
                });
            }
        }

        [HttpPost]
        public ActionResult Edit(Guid id, RegionalServerView model, FormCollection formCollection)
        {
            try
            {
                using(var context = new SyncMetadataEntities())
                {
                    var i = context.MetaCompanies.FirstOrDefault(j => j.Id == id);
                    if(i != null)
                    {
                        if(i.MachineKey != model.HWKey)
                        {
                            TrackChanges("Аппаратный ключ", i.Id, i.MachineKey, model.HWKey);
                            i.MachineKey = model.HWKey;
                        }
                        if(i.EmailFailure != model.EmailFailure)
                        {
                            TrackChanges("Получатели писем о сбоях", i.Id, i.EmailFailure, model.EmailFailure);
                            i.EmailFailure = model.EmailFailure;
                        }
                        if(i.EmailSuccess != model.EmailSuccess)
                        {
                            TrackChanges("Получатели писем о успешной синхронизации", i.Id, i.EmailSuccess, model.EmailSuccess);
                            i.EmailSuccess = model.EmailSuccess;
                        }
                        if(i.LicenceTillUncommited != model.LicenseTillUncommited)
                        {
                            TrackChanges("Срок действия лицензии неодобренный", i.Id, i.LicenceTillUncommited, model.LicenseTillUncommited);
                            i.LicenceTillUncommited = model.LicenseTillUncommited;
                        }
                        if(i.MaxSyncPeriod != model.MaxSyncPeriod)
                        {
                            TrackChanges("Срок действия ключа", i.Id, i.MaxSyncPeriod, model.MaxSyncPeriod);
                            i.MaxSyncPeriod = model.MaxSyncPeriod;
                        }
                        if(i.IsLicenseAvailable != (formCollection["IsLicenseAvailable"] == "on"))
                        {
                            TrackChanges("Доступность ключа лицензии", i.Id, i.IsLicenseAvailable, formCollection["IsLicenseAvailable"] == "on");
                            i.MaxSyncPeriod = model.MaxSyncPeriod;
                        }
                        if(i.ContactPerson != model.ContactPerson)
                        {
                            TrackChanges("Контактное лицо", i.Id, i.ContactPerson, model.ContactPerson);
                            i.ContactPerson = model.ContactPerson;
                        }
                        if(i.SendSms != (formCollection["SendSms"] == "on"))
                        {
                            TrackChanges("Отправка СМС клиентам", i.Id, i.SendSms, formCollection["SendSms"] == "on");
                            i.SendSms = formCollection["SendSms"] == "on";
                        }
                        if(i.SmsUntil != model.SmsUntil)
                        {
                            TrackChanges("Отправка СМС клиентам - дата", i.Id, i.SmsUntil, model.SmsUntil);
                            i.SmsUntil = model.SmsUntil;
                        }

                        if(i.SmsMarketing != (formCollection["SmsMarketing"] == "on"))
                        {
                            TrackChanges("SmsMarketing", i.Id, i.SmsMarketing, formCollection["SmsMarketing"] == "on");
                            i.SmsMarketing = formCollection["SmsMarketing"] == "on";
                        }
                        if(i.SmsBirthday != (formCollection["SmsBirthday"] == "on"))
                        {
                            TrackChanges("SmsBirthday", i.Id, i.SmsBirthday, formCollection["SmsBirthday"] == "on");
                            i.SmsBirthday = formCollection["SmsBirthday"] == "on";
                        }
                        if(i.SmsTreatments != (formCollection["SmsTreatments"] == "on"))
                        {
                            TrackChanges("SmsTreatments", i.Id, i.SmsTreatments, formCollection["SmsTreatments"] == "on");
                            i.SmsTreatments = formCollection["SmsTreatments"] == "on";
                        }


                        context.SaveChanges();
                    }
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        private void TrackChanges(string fieldName, Guid objectId, object oldValue, object newValue)
        {
            using(var context = new SyncMetadataEntities())
            {
                context.Changelog.AddObject(new Changelog
                {
                    Id = Guid.NewGuid(),
                    Date = DateTime.Now,
                    UserName = User.Identity.Name,
                    Field = fieldName,
                    ObjectId = objectId,
                    OldValue = oldValue != null ? oldValue.ToString() : null,
                    NewValue = newValue != null ? newValue.ToString() : null
                });
                context.SaveChanges();
            }
        }


        public void GetExcel(string typeId, string start, string end, string filterId, string emplId, string repId, string depId)
        {
            Response.AddHeader("Content-Disposition", "attachment; filename=РегСервера.xls");
            Response.ContentType = "text/csv";
            Response.ContentEncoding = System.Text.Encoding.GetEncoding("utf-8");
            Response.Write(GetTable());
            Response.End();
        }

        private StringWriter GetTable()
        {
            using(var main = new TonusEntities())
            using(var context = new SyncMetadataEntities())
            {

                var data = context.MetaCompanies
                         .ToList().Select(i => new RegionalServerView
                     {
                         CompanyName = main.Companies.Single(j => j.CompanyId == i.CompanyId).CompanyName,
                         DivisionName = main.Divisions.Single(j => j.Id == i.DivisionId).Name,
                         ContactPerson = i.ContactPerson,
                         LastSync = i.LastSyncDate,
                         LastKeyReceived = i.LastKeyReceived,
                         MaxSyncPeriod = i.MaxSyncPeriod ?? 0,
                         Id = i.Id,
                         LicenseTill = i.LicenseTill,
                         LicenseTillUncommited = i.LicenceTillUncommited,
                         IsLicenseAvailable = i.IsLicenseAvailable,
                         SmsMarketing = i.SmsMarketing,
                         SmsBirthday = i.SmsBirthday,
                         SmsTreatments = i.SmsTreatments
                     }
                     ).ToList();
                StringWriter sw = new StringWriter();


                sw.WriteLine("<table cellspacing=0>");

                sw.WriteLine(@"<tr><th style='border:1px solid black'>Франчайзи</th>
            <th style='border:1px solid black'>Клуб</th>
            <th style='border:1px solid black'>Срок лицензии</th>
            <th style='border:1px solid black'>Последняя синхронизация</th>
            <th style='border:1px solid black'>Последняя загрузка ключа</th>
            <th style='border:1px solid black'>Длительность ключа</th>");

                foreach(var item in data)
                {
                    sw.WriteLine(String.Format(@"<tr>
            <td style='border:1px solid black'>{0}</td>
            <td style='border:1px solid black'>{1}</td>
            <td style='border:1px solid black'>{2}</td>
            <td style='border:1px solid black'>{3}</td>
            <td style='border:1px solid black'>{4}</td>
            <td style='border:1px solid black'>{5}</td></tr>",
                                               item.CompanyName,
                                               item.DivisionName,
                                               item.LicenseTill,
                                               item.LastSync,
                                               item.LastKeyReceived,
                                               item.MaxSyncPeriod));
                }

                sw.WriteLine("</table>");

                return sw;
            }
        }


    }
}