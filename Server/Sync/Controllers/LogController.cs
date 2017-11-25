using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sync.Models;
using TonusClub.Entities;

namespace Sync.Controllers
{
    public class LogController : Controller
    {
        public ActionResult Index(Guid id)
        {
            var context = new SyncMetadataEntities();

            var comp = context.MetaCompanies.First(i => i.Id == id);
            var date = DateTime.Today.AddMonths(-1);
            var logs = context.LogItems.Where(i => i.MetaCompanyId == comp.DivisionId && i.CreatedOn > date).OrderByDescending(i=>i.CreatedOn);

            var div = new TonusEntities().Divisions.Single(i => i.Id == comp.DivisionId);

            ViewBag.CompanyName = div.Company.CompanyName;
            ViewBag.DivisionName = div.Name;

            return View(logs);
        }

        public ActionResult Details(Guid id)
        {
            var context = new SyncMetadataEntities();

            var log = context.LogItems.First(i => i.Id == id);
            var comp = context.MetaCompanies.First(i => i.DivisionId == log.MetaCompanyId);

            var div = new TonusEntities().Divisions.Single(i => i.Id == comp.DivisionId);

            ViewBag.CompanyName = div.Company.CompanyName;
            ViewBag.DivisionName = div.Name;

            return View(log);

        }
    }
}
