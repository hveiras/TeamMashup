using System;
using System.Linq;
using System.Web.Mvc;
using TeamMashup.Core.Domain;
using TeamMashup.Core.Mailing;
using TeamMashup.Core.Security;
using TeamMashup.Models.Public;
using TeamMashup.Server.Filters;
using TMSubscription = TeamMashup.Core.Domain.Subscription;

namespace TeamMashup.Server.Areas.Public.Controllers
{
    [WebAuthorize]
    [NoCache]
    public class AccountController : PublicBaseController
    {
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SendRestorePasswordEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return View("Error");

            try
            {
                using (var context = new DatabaseContext())
                {
                    User user;
                    if (!context.Users.TryGetByEmail(email, out user))
                    {
                        //TODO: Log Error.
                        return View("Error");
                    }

                    //TODO: enforce the requirement that the email address is unique across the whole platform.
                    //If a user has two subscriptions, then he has to use two differente emails.
                    TMSubscription subscription;
                    if (!context.Subscriptions.TryGetBySubscriptor(user, out subscription))
                    {
                        //The user that rquested password recovery exists on the database but he is not a subscriptor.

                        //TODO: Log Error.
                        return View("Error");
                    }

                    var token = SecurityManager.GenerateToken();

                    var recovery = new PasswordRecovery
                    {
                        Code = token.Code,
                        SubscriptionId = subscription.Id,
                        Expires = token.Expires
                    };

                    context.PasswordRecoveries.Add(recovery);
                    context.SaveChanges();

                    string mailBody = recovery.Code.ToString();
                    string from = string.Empty;
                    string to = string.Empty;

                    //The subscriptor will receive an email with a link including the security code to restore his account.
                    EmailHelper.Send(mailBody, to, from);

                    return View("PasswordRecoveryEmailSent");
                }
            }
            catch (Exception)
            {
                //TODO: Log exception
                return View("Error");
            }
        }

        public ActionResult RestorePassword(string recoveryLink)
        {
            if (string.IsNullOrWhiteSpace(recoveryLink))
                return View("Error");

            try
            {
                using (var context = new DatabaseContext())
                {
                    //recoveryLink format: http://teammashup.com/signin/restorepassword?token=2456C5CE-E935-434A-962B-DD9675A688B4
                    Guid token;
                    if (!SecurityManager.TryGetToken(recoveryLink, out token))
                    {
                        //TODO: Log Error.
                        return View("Error");
                    }

                    PasswordRecovery recovery;
                    if (!context.PasswordRecoveries.TryGetByCode(token, out recovery))
                    {
                        //TODO: Log Error.
                        return View("Error");
                    }

                    if (recovery.IsExpiredOrClaimed())
                    {
                        //TODO: Log Error.
                        return View("Error");
                    }

                    var model = new RestorePasswordViewDto();
                    return View(model);
                }
            }
            catch (Exception)
            {
                //TODO: Log exception
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult ConfirmRestore(RestorePasswordViewDto dto)
        {
            if (dto == null)
                return View("Error");

            try
            {
                using (var context = new DatabaseContext())
                {
                    PasswordRecovery recovery;
                    if (!context.PasswordRecoveries.TryGetByCode(dto.Token, out recovery))
                    {
                        //TODO: Log Error.
                        return View("Error");
                    }

                    if (recovery.IsExpiredOrClaimed())
                    {
                        //TODO: Log Error.
                        return View("Error");
                    }

                    var subscription = context.Subscriptions.Single(x => x.Id == recovery.SubscriptionId);
                    var user = context.Users.Single(x => x.Id == subscription.Subscriptor.Id);

                    recovery.Claimed = true;
                    user.Password = dto.NewPassword;

                    context.SaveChanges();
                }

                return RedirectToAction("SignIn");
            }
            catch (Exception)
            {
                //TODO: Log exception
                return View("Error");
            }
        }
    }
}