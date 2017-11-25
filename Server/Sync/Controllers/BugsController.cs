using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sync.Models;
using Sync.Code;

namespace Sync.Controllers
{
    [Authorize]
    public class BugsController : Controller
    {
        public ActionResult Index(bool showAll = false)
        {
            ViewBag.ShowAll = showAll;

            using (var context = new SyncMetadataEntities())
            {
                return View(context.Bugs.Where(i => showAll || i.StatusId != 4).OrderBy(b => b.PriorityId).ThenBy(b => b.Id).ToList());
            }
        }

        public ActionResult Details(int id)
        {
            using (var context = new SyncMetadataEntities())
            {
                var bug = context.Bugs.SingleOrDefault(b => b.Id == id);
                if (bug == null)
                {
                    return RedirectToAction("Index");
                }
                return View(bug);
            }
        }

        public ActionResult Create()
        {
            FillViewBag();
            return View();
        }

        private void FillViewBag()
        {
            ViewBag.Categories = Sync.Models.Enumerators.Categories.Select(i => new SelectListItem { Text = i.Value, Value = i.Key.ToString() });
            ViewBag.Priorities = Sync.Models.Enumerators.Priorities.Select(i => new SelectListItem { Text = i.Value, Value = i.Key.ToString() });
        } 

        [HttpPost]
        public ActionResult Create(Bug bug, FormCollection collection)
        {
            using (var context = new SyncMetadataEntities())
            {
                bug.CreatedBy = UserManager.GetCurrentUserId(this);
                bug.CreatedOn = DateTime.Now;
                bug.StatusChangedOn = DateTime.Now;
                bug.StatusId = 0;
                context.Bugs.AddObject(bug);
                context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            using (var context = new SyncMetadataEntities())
            {
                var bug = context.Bugs.SingleOrDefault(b => b.Id == id);
                if (bug == null)
                {
                    return RedirectToAction("Index");
                }
                FillViewBag();
                ((IEnumerable<SelectListItem>)ViewBag.Categories).Single(i => i.Value == bug.CategoryId.ToString()).Selected = true;
                ((IEnumerable<SelectListItem>)ViewBag.Priorities).Single(i => i.Value == bug.PriorityId.ToString()).Selected = true;
                return View(bug);
            }
        }

        [HttpPost]
        public ActionResult Edit(int id, Bug bug, FormCollection collection)
        {
            using (var context = new SyncMetadataEntities())
            {
                var org = context.Bugs.SingleOrDefault(b => b.Id == id);
                if (org == null)
                {
                    return View(bug);
                }
                if (org.StatusId == 1)
                {
                    org.StatusId = 0;
                    org.StatusChangedOn = DateTime.Now;
                }
                if (org.StatusId == 0 || org.StatusId == 1)
                {
                    org.Subject = bug.Subject;
                    org.Message = bug.Message;
                    org.PriorityId = bug.PriorityId;
                    org.CategoryId = bug.CategoryId;

                    context.SaveChanges();
                }

                return RedirectToAction("Index");
            }

        }

        public ActionResult Start(int id)
        {
            if (User.Identity.Name == "max")
            {
                using (var context = new SyncMetadataEntities())
                {
                    var bug = context.Bugs.SingleOrDefault(b => b.Id == id);
                    if (bug != null)
                    {
                        bug.StatusChangedOn = DateTime.Now;
                        bug.StatusId = 2;
                        context.SaveChanges();
                    }
                }
            }
            return RedirectToAction("Index");
        }

        public ActionResult Cancel(int id)
        {
            if (User.Identity.Name != "max")
            {
                return RedirectToAction("Index");
            }
            using (var context = new SyncMetadataEntities())
            {
                var bug = context.Bugs.SingleOrDefault(b => b.Id == id);
                if (bug == null)
                {
                    return RedirectToAction("Index");
                }
                return View(bug);
            }
        }

        [HttpPost]
        public ActionResult Cancel(int id, Bug bug, FormCollection collection)
        {
            if (User.Identity.Name != "max")
            {
                return RedirectToAction("Index");
            }

            using (var context = new SyncMetadataEntities())
            {
                var org = context.Bugs.SingleOrDefault(b => b.Id == id);
                if (org == null)
                {
                    return View(bug);
                }
                org.StatusId = 1;
                org.StatusChangedOn = DateTime.Now;
                org.Resolution = bug.Resolution;

                context.SaveChanges();

                return RedirectToAction("Index");
            }

        }

        public ActionResult Close(int id)
        {
                using (var context = new SyncMetadataEntities())
                {
                    var bug = context.Bugs.SingleOrDefault(b => b.Id == id);
                    if (bug != null)
                    {
                        bug.StatusChangedOn = DateTime.Now;
                        bug.StatusId = 4;
                        context.SaveChanges();
                    }
                }
            return RedirectToAction("Index");
        }

        public ActionResult Finish(int id)
        {
            if (User.Identity.Name != "max")
            {
                return RedirectToAction("Index");
            }
            using (var context = new SyncMetadataEntities())
            {
                var bug = context.Bugs.SingleOrDefault(b => b.Id == id);
                if (bug == null)
                {
                    return RedirectToAction("Index");
                }
                return View(bug);
            }
        }

        [HttpPost]
        public ActionResult Finish(int id, Bug bug, FormCollection collection)
        {
            if (User.Identity.Name != "max")
            {
                return RedirectToAction("Index");
            }

            using (var context = new SyncMetadataEntities())
            {
                var org = context.Bugs.SingleOrDefault(b => b.Id == id);
                if (org == null)
                {
                    return View(bug);
                }
                org.StatusId = 3;
                org.StatusChangedOn = DateTime.Now;
                org.Resolution = bug.Resolution;

                context.SaveChanges();

                return RedirectToAction("Index");
            }

        }

        public ActionResult Reopen(int id)
        {
            using (var context = new SyncMetadataEntities())
            {
                var bug = context.Bugs.SingleOrDefault(b => b.Id == id);
                if (bug == null)
                {
                    return RedirectToAction("Index");
                }
                return View(bug);
            }
        }

        [HttpPost]
        public ActionResult Reopen(int id, Bug bug, FormCollection collection)
        {
            using (var context = new SyncMetadataEntities())
            {
                var org = context.Bugs.SingleOrDefault(b => b.Id == id);
                if (org == null)
                {
                    return View(bug);
                }
                org.StatusId = 1;
                org.StatusChangedOn = DateTime.Now;
                org.Message = bug.Message;

                context.SaveChanges();

                return RedirectToAction("Index");
            }

        }


    }
}
