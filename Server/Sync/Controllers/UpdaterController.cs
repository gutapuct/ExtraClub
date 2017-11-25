using System;
using System.Linq;
using System.Web.Mvc;
using System.IO;
using System.Text;
using Sync.Models;

namespace Sync.Controllers
{
    public class UpdaterController : Controller
    {
        public ActionResult GetCurrentDBVersion()
        {
            var di = new DirectoryInfo("c:\\asu\\dbscripts");

            if (!di.Exists) return Content("-1");
            var files = di.GetFiles("*.sql");
            var maxversion = -1;
            foreach (var fi in files)
            {
                int i;
                var fn = fi.Name.Replace(".sql", "");
                if (Int32.TryParse(fn, out i))
                {
                    maxversion = Math.Max(maxversion, i);
                }
            }
            return Content(maxversion.ToString());
        }

        public ActionResult GetCurrentDBVersionEx()
        {
            return Content(GetVersionNumber("dbupdates").ToString());
        }

        public ActionResult GetCurrentWSVersion()
        {
            return Content(GetVersionNumber("wsupdates").ToString());
        }

        public ActionResult GetCurrentWFVersion()
        {
            return Content(GetVersionNumber("wfupdates").ToString());
        }

        public ActionResult GetCurrentUpdVersion()
        {
            return Content(GetVersionNumber("updupdates").ToString());
        }

        public ActionResult Ping(string id)
        {
            try
            {
                var did = Guid.Parse(id);
                var context = new SyncMetadataEntities();
                var c = context.MetaCompanies.FirstOrDefault(i => i.DivisionId == did);
                if (c != null)
                {
                    c.LastUpdated = DateTime.Now;
                    context.SaveChanges();
                }
            }
            catch { }
            return Content("OK");
        }


        public int GetVersionNumber(string folder)
        {
            var di = new DirectoryInfo(Path.Combine("c:\\asu", folder));

            if (!di.Exists) return -1;
            var files = di.GetFiles("*.zip");
            var maxversion = -1;
            foreach (var fi in files)
            {
                int i;
                var fn = fi.Name.Replace(".zip", "");
                if (Int32.TryParse(fn, out i))
                {
                    maxversion = Math.Max(maxversion, i);
                }
            }
            return maxversion;
        }

        public ActionResult GetDBScript(int id)
        {
            var fi = new FileInfo(String.Format("c:\\asu\\dbscripts\\{0}.sql", id));

            if (!fi.Exists) return Content("");
            var sr = new StreamReader(fi.FullName, Encoding.GetEncoding(1251));
            var str = sr.ReadToEnd();
            sr.Close();
            return Content(str);
        }

        public FileResult GetCurrentWS()
        {
            var fi = new FileInfo(String.Format("c:\\asu\\wsupdates\\{0}.zip", GetVersionNumber("wsupdates")));

            if(!fi.Exists)
                return File(new byte[0], System.Net.Mime.MediaTypeNames.Application.Zip);
            var res = System.IO.File.ReadAllBytes(fi.FullName);
            return File(res, System.Net.Mime.MediaTypeNames.Application.Zip);
        }

        public FileResult GetCurrentDb()
        {
            var fi = new FileInfo(String.Format("c:\\asu\\dbupdates\\{0}.zip", GetVersionNumber("dbupdates")));

            if(!fi.Exists)
                return File(new byte[0], System.Net.Mime.MediaTypeNames.Application.Zip);
            var res = System.IO.File.ReadAllBytes(fi.FullName);
            return File(res, System.Net.Mime.MediaTypeNames.Application.Zip);
        }

        public FileResult GetCurrentWF()
        {
            var fi = new FileInfo(String.Format("c:\\asu\\wfupdates\\{0}.zip", GetVersionNumber("wfupdates")));

            if (!fi.Exists) return File(new byte[0], System.Net.Mime.MediaTypeNames.Application.Zip);
            var res = System.IO.File.ReadAllBytes(fi.FullName);
            return File(res, System.Net.Mime.MediaTypeNames.Application.Zip);
        }

        public FileResult GetCurrentUpd()
        {
            var fi = new FileInfo(String.Format("c:\\asu\\updupdates\\{0}.zip", GetVersionNumber("updupdates")));

            if (!fi.Exists) return File(new byte[0], System.Net.Mime.MediaTypeNames.Application.Zip);
            var res = System.IO.File.ReadAllBytes(fi.FullName);
            return File(res, System.Net.Mime.MediaTypeNames.Application.Zip);
        }

        public ActionResult Success(string ClubId, string Part, string Version)
        {
            using (var context = new SyncMetadataEntities())
            {
                Guid clubId;
                if (Guid.TryParse(ClubId, out clubId))
                {
                    var club = context.MetaCompanies.FirstOrDefault(i => i.DivisionId == clubId);
                    if (club != null)
                    {
                        if (Part == "Wf")
                        {
                            club.WfVersion = Version;
                        }
                        else if (Part == "Ws")
                        {
                            club.WsVersion = Version;
                        }
                        else if (Part == "Db")
                        {
                            club.DbVersion = Version;
                        }
                        else if (Part == "Upd")
                        {
                            club.UpdVersion = Version;
                        }
                        club.LastUpdated = DateTime.Now;
                        context.SaveChanges();
                    }
                }
                return Content("");
            }
        }
    }
}
