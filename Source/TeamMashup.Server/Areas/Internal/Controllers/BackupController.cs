using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TeamMashup.Core.Contracts;
using TeamMashup.Core.Domain;
using TeamMashup.Core.Enums;
using TeamMashup.Models.Internal;
using TeamMashup.Server.Filters;

namespace TeamMashup.Server.Areas.Internal.Controllers
{
    [NoCache]
    public class BackupController : InternalBaseController
    {
        public ActionResult Backups()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult AddBackupRequest()
        {
            return PartialView("_AddBackupRequest");
        }

        [HttpPost]
        public ActionResult ScheduleBackupRequest()
        {
            if (ModelState.IsValid)
            {
                var request = new BackupRequest(BackupType.Full, BackupSchedule.OnDemand);

                Context.AddBackupRequest(request);
                Context.SaveChanges();
            }

            return PartialView("_AddBackupRequest");
        }

        public ActionResult GetBackupRequestItems(int iDisplayStart, int iDisplayLength, string sEcho)
        {
            using (var context = new DatabaseContext())
            {
                var query = context.BackupRequests.OrderByDescending(x => x.CreatedDate);

                var totalRecords = query.Count();

                var page = query.Skip(iDisplayStart).Take(iDisplayLength).ToList();

                var model = new DataTablePage
                {
                    sEcho = sEcho,
                    iTotalRecords = totalRecords,
                    iTotalDisplayRecords = totalRecords
                };

                var backupRequests = (from x in page
                             select new BackupRequestModel
                             {
                                 Path = x.Path,
                                 Schedule = x.Schedule,
                                 State = x.State,
                                 Type = x.Type,
                                 CreatedDate = x.CreatedDate
                             }).ToList();

                foreach (var backup in backupRequests)
                {
                    var item = new Dictionary<string, string>
                    {
                        {"Path", backup.Path},
                        {"Schedule", backup.Schedule.ToString()},
                        {"State", backup.State.ToString()},
                        {"Type", backup.Type.ToString()},
                        {"CreatedDate", backup.CreatedDate.ToString()},
                        {"DT_RowId", "roleItem_" + backup.Id}
                    };

                    model.aaData.Add(item);
                }

                return Json(model, JsonRequestBehavior.AllowGet);
            }
        }
    }
}