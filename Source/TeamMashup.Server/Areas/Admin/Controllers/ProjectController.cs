using Microsoft.Web.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TeamMashup.Core.Contracts;
using TeamMashup.Core.Domain;
using TeamMashup.Membership;
using TeamMashup.Models.Admin;
using TeamMashup.Server.Filters;

namespace TeamMashup.Server.Areas.Admin.Controllers
{
    [NoCache]
    public class ProjectController : TenantBaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetProjectItems(int iDisplayStart, int iDisplayLength, string sEcho, string sSearch)
        {
            int totalRecords;
            var query = Context.Projects.Search(iDisplayStart, iDisplayLength, sSearch, out totalRecords);

            var projects = (from x in query.Where(x => !x.IsEnded)
                            select new ProjectModel
                            {
                                Id = x.Id,
                                Name = x.Name
                            }).ToList();

            var model = new DataTablePage
            {
                sEcho = sEcho,
                iTotalRecords = totalRecords,
                iTotalDisplayRecords = totalRecords
            };

            foreach (var project in projects)
            {
                var item = new Dictionary<string, string>
                    {
                        {"Name", project.Name},
                        {"DT_RowId", "roleItem_" + project.Id}
                    };

                model.aaData.Add(item);
            }

            return Json(model, JsonRequestBehavior.AllowGet);

        }

        public ActionResult NewProject()
        {
            ViewBag.Title = "New Project";

            var roles = Context.Database.ProjectAssignmentRoles.ToList();
            ViewBag.Roles = JsonConvert.SerializeObject(roles);

            var model = new ProjectAddModel();

            return View("Project", model);
        }

        [HttpPost]
        public ActionResult AddProject(ProjectAddModel model)
        {
            if(ModelState.IsValid)
            {
                if (!Context.Projects.Exists(model.Name))
                {
                    var assignments = model.SelectedMembers.Select(x => new ProjectAssignment
                    {
                        RoleId = x.RoleId,
                        UserId = x.UserId
                    }).ToList();

                    Context.Database.Projects.Add(new Project(WebSecurity.CurrentUserSubscriptionId, model.Name)
                    {
                        Description = model.Description,
                        Assignments = assignments
                    });

                    Context.SaveChanges();
                }
                else
                { 
                    //Add error to model and show.
                }
            }

            return this.RedirectToAction(x => x.Index());
        }
    }
}