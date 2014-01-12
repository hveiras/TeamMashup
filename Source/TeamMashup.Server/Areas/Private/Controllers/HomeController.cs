using Microsoft.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TeamMashup.Core.Domain;
using TeamMashup.Core.Enums;
using TeamMashup.Membership;
using TeamMashup.Models.Admin;
using TeamMashup.Models.Internal;
using TeamMashup.Models.Private;
using TeamMashup.Server.Filters;

namespace TeamMashup.Server.Areas.Private.Controllers
{
    [NoCache]
    public class HomeController : TenantBaseController
    {
        [Authorize]
        public ActionResult Index()
        {
            var user = Context.Users.GetById(WebSecurity.CurrentUserId);
            var userProjectIds = user.Assignments.Select(a => a.ProjectId).ToList();

            var model = new HomeModel
            {
                YourProjects = Context.Projects.FilterByIds(userProjectIds).Select(x => new ProjectModel
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToList()
            };

            ViewBag.ShowChatCollapsed = false;

            return View(model);
        }

        public ActionResult GetComments(int iDisplayStart, int iDisplayLength, string sEcho)
        {
            var query = Context.Comments.AsPredicateTrue()
                        .And(x => x.ScopeValue == (int)CommentScope.Tenant)
                        .Evaluate()
                        .OrderByDescending(x => x.CreatedDate);

            var page = query.GetPage(iDisplayStart, iDisplayLength);

            var model = new CommentPageModel();

            foreach (var comment in page.Items)
            {
                if (comment.Replies == null)
                    comment.Replies = new List<CommentReply>();

                var user = Context.Users.GetById(comment.UserId);

                var replies = (from r in comment.Replies
                               let replyUser = Context.Users.GetById(r.UserId)
                               select new CommentReplyModel
                               {
                                   Id = r.Id,
                                   UserId = r.UserId,
                                   UserName = replyUser.Name,
                                   CreatedDate = r.CreatedDate,
                                   Message = r.Message
                               }).ToList();

                model.Comments.Add(new CommentModel
                {
                    Id = comment.Id,
                    UserId = comment.UserId,
                    UserName = user.Name,
                    Message = comment.Message,
                    Replies = replies
                });
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddComment()
        {
            return PartialView("_AddComment");
        }

        [HttpPost]
        public ActionResult AddComment(Comment model)
        {
            if (ModelState.IsValid)
            {
                Context.Database.Comments.Add(new Comment(WebSecurity.CurrentUserId, WebSecurity.CurrentUserSubscriptionId, model.Message, CommentScope.Tenant));
                Context.SaveChanges();
            }

            return PartialView("_AddComment", model);
        }

        [HttpPost]
        public ActionResult AddReply(CommentReplyModel model)
        {
            if (ModelState.IsValid)
            {
                var comment = Context.Comments.GetById(model.CommentId);

                comment.Replies.Add(new CommentReply
                {
                    CreatedDate = DateTime.UtcNow,
                    Message = model.Message,
                    UserId = WebSecurity.CurrentUserId
                });

                Context.SaveChanges();

                return JsonSuccess();
            }

            return JsonError();
        }

        [ChildActionOnly]
        public ActionResult Surveys()
        {
            var model = new List<SurveyModel>();

            var profile = Context.Database.UserProfiles.SingleOrDefault(x => x.UserId == WebSecurity.CurrentUserId);

            if (profile != null)
            {
                var completedSurveyIds = profile.CompletedSurveys.Select(x => x.Id).ToList();

                var surveys = (from x in Context.Database.Surveys
                               where x.Active && !completedSurveyIds.Contains(x.Id)
                               select x).ToList();

                model = surveys.Select(x => new SurveyModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    Options = x.Options.Select(y => new SurveyItemModel { Id = y.Id, Description = y.Description, SurveyId = y.SurveyId, Votes = y.Votes }).ToList()
                }).ToList();
            }

            return PartialView("_Surveys", model);
        }

        [HttpPost]
        public ActionResult SurveyVote(SurveyVoteModel model)
        {
            if (ModelState.IsValid)
            {
                SurveyItem surveyItem;
                if (!Context.Database.SurveyItems.TryGetById(model.OptionId, out surveyItem))
                    throw new InvalidOperationException(string.Format("survey item with id {0} was not found", model.SurveyId));

                surveyItem.Votes += 1;

                var profile = Context.Database.UserProfiles.SingleOrDefault(x => x.UserId == WebSecurity.CurrentUserId);

                if (profile != null)
                {
                    Survey survey;
                    if (!Context.Database.Surveys.TryGetById(model.SurveyId, out survey))
                        throw new InvalidOperationException(string.Format("survey with id {0} was not found", model.SurveyId));

                    profile.CompletedSurveys.Add(survey);
                }

                Context.SaveChanges();
            }

            return this.RedirectToAction(x => x.Index());
        }
    }
}