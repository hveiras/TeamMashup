using Microsoft.Web.Mvc;
using System;
using System.Linq;
using System.Web.Mvc;
using TeamMashup.Core.Domain;
using TeamMashup.Membership;
using TeamMashup.Models.Private;
using TeamMashup.Server.Filters;

namespace TeamMashup.Server.Areas.Private.Controllers
{
    [NoCache]
    public class AccountController : TenantBaseController
    {
        public ActionResult Index()
        {
            var profile = Context.Database.UserProfiles.SingleOrDefault(x => x.UserId == WebSecurity.CurrentUserId);

            if (profile == null)
            {
                //TODO: use the existing language set on UI to create the default profile.
                var language = Context.Database.Languages.GetByName("English");
                profile = new UserProfile(WebSecurity.CurrentUserId, language.Id);
                Context.Database.UserProfiles.Add(profile);
                Context.SaveChanges();
            }

            var model = new UserProfileModel
            {
                Bio = profile.Bio,
                BirthDay = profile.BirthDay.HasValue ? profile.BirthDay : new DateTime?(),
                Department = profile.Department,
                Expertise = profile.Expertise,
                FacebookProfile = profile.FacebookProfile,
                SkypeName = profile.SkypeName,
                Interests = profile.Interests,
                JobTitle = profile.JobTitle,
                Languages = new SelectList(Context.Database.Languages.ToList(), "Id", "Name"),
                LanguageId = profile.LanguageId,
                LinkedinProfile = profile.LinkedinProfile,
                Location = profile.Location,
                MobilePhone = profile.MobilePhone,
                TwitterUserName = profile.TwitterUserName,
                WorkPhone = profile.WorkPhone,
                WorkPhoneExtension = profile.WorkPhoneExtension
            };

            return View("Account", model);
        }

        public ActionResult Logout()
        {
            WebSecurity.Logout();

            return RedirectToAction("Index", "Home", new { area = "Public" });
        }

        [HttpPost]
        public ActionResult Update(UserProfileModel model)
        {
            var profile = Context.Database.UserProfiles.Single(x => x.UserId == WebSecurity.CurrentUserId);

            profile.LanguageId = model.LanguageId;

            Context.SaveChanges();

            return this.RedirectToAction(x => x.Index());
        }
    }
}