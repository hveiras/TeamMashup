using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TeamMashup.Core.Contracts;
using TeamMashup.Core.Domain;
using TeamMashup.Server.Filters;

namespace TeamMashup.Server.Areas.Internal.Controllers
{
    [WebAuthorize(Claims="read-log")]
    [NoCache]
    public class TrackingController : InternalBaseController
    {
        public ActionResult LogViewer()
        {
            return View();
        }

        public ActionResult GetLogItems(int iDisplayStart, int iDisplayLength, string sEcho, string sSearch)
        {
            int totalResults;
            var results = Context.Logs.Search(iDisplayStart, iDisplayLength, sSearch, out totalResults).ToList();

            var model = new DataTablePage
            {
                sEcho = sEcho,
                iTotalRecords = totalResults,
                iTotalDisplayRecords = totalResults
            };

            foreach (var log in results)
            {
                var item = new Dictionary<string, string>
                    {
                        {"Message", log.Message},
                        {"DT_RowId", "roleItem_" + log.Id}
                    };

                model.aaData.Add(item);
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}
