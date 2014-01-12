using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TeamMashup.Core.Domain;
using TeamMashup.Core.Enums;
using TeamMashup.Membership;
using TeamMashup.Models.Internal;
using TeamMashup.Server.Filters;

namespace TeamMashup.Server.Areas.Internal.Controllers
{
    [WebAuthorize]
    [NoCache]
    public class DashboardController : InternalBaseController
    {
        public ActionResult Index()
        {
            ViewBag.ShowChatCollapsed = false;
            return View("Dashboard");
        }

        public ActionResult GetComments(int iDisplayStart, int iDisplayLength, string sEcho)
        {
            var query = from c in Context.Comments
                        where c.ScopeValue == (int)CommentScope.Internal
                        orderby c.CreatedDate descending
                        select c;

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
                Context.Comments.Add(new Comment
                {
                    UserId = WebSecurity.CurrentUserId,
                    Message = model.Message,
                    Scope = CommentScope.Internal,
                    CreatedDate = DateTime.UtcNow
                });
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

    }
}
