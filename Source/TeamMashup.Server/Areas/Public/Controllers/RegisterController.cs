using Microsoft.Web.Mvc;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using TeamMashup.Core;
using TeamMashup.Core.Domain;
using TeamMashup.Core.Enums;
using TeamMashup.Membership;
using TeamMashup.Models.Public;
using TeamMashup.Server.Filters;
using TMSubscription = TeamMashup.Core.Domain.Subscription;

namespace TeamMashup.Server.Areas.Public.Controllers
{
    [AllowAnonymous]
    [NoCache]
    public class RegisterController : PublicBaseController
    {
        [AllowAnonymous]
        public ActionResult SelectPlan()
        {
            using (var context = new DatabaseContext())
            {
                var plans = context.SubscriptionPlans.NonDeleted().Select(x => new SubscriptionPlanModel
                    {
                        Id = x.Id,
                        Description = x.Description,
                        Name = x.Name
                    }).ToList();

                var model = new SelectPlanModel
                {
                    Plans = plans
                };

                return View(model);
            }
        }

        [AllowAnonymous]
        public ActionResult CreateSubscription(long planId)
        {
            using (var context = new DatabaseContext())
            {
                SubscriptionPlan plan;
                if (!context.SubscriptionPlans.TryGetById(planId, out plan))
                    throw new ApplicationException("plan with id = " + planId + " was not found!");

                var countries = context.Countries.ToList();

                var model = new CreateSubscriptionModel
                {
                    PlanId = plan.Id,
                    PlanName = plan.Name,
                    Countries = new SelectList(countries, "Id", "Name")
                };

                return View(model);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult CreateSubscription(CreateSubscriptionModel model)
        {
            using (var context = new DatabaseContext())
            {
                string tenantName;
                if (!StringExtensions.TryParseTenantName(model.Email, out tenantName))
                    throw new InvalidOperationException("cannot parse tenant name from email");

                if (context.Subscriptions.Any(x => x.TenantName.Equals(tenantName, StringComparison.InvariantCultureIgnoreCase)))
                {
                    ModelState.AddModelError("email", string.Format("Company name: {0} is already registered", tenantName));
                    model.Countries = new SelectList(context.Countries.ToList(), "Id", "Name");
                    return View(model);
                }

                string emailDomain;
                if (!StringExtensions.TryParseEmailDomain(model.Email, out emailDomain))
                    throw new InvalidOperationException("cannot parse domain name from email");

                var subscription = new TMSubscription(model.CompanyName, tenantName, model.Email, emailDomain, model.PlanId, model.CountryId, model.CompanyAddress)
                {
                    BillingAddress = model.BillingAddress,
                    CreditCardNumber = model.CreditCardNumber,
                    CreditCardExpireDate = model.CreditCardExpireDate,
                    SecurityCode = model.CreditCardSecurityCode,
                };

                context.Subscriptions.Add(subscription);
                context.SaveChanges();

                MembershipCreateStatus status;
                var subscriptor = WebSecurity.Membership.CreateUser(subscription.Id, model.Name, model.Email, model.Password, out status);

                if (status != MembershipCreateStatus.Success)
                    throw new MembershipCreateUserException(status);

                subscription.SubscriptorId = subscriptor.Id;
                subscription.State = SubscriptionState.Active;
                context.SaveChanges();

                WebSecurity.Login(subscriptor.Email, model.Password);

                return this.RedirectToAction<RegisterController>(x => x.Congratulations());

            }
        }

        public ActionResult Congratulations()
        {
            using (var context = new DatabaseContext())
            {
                var subscription = context.Subscriptions.GetById(WebSecurity.CurrentUserSubscriptionId);

                ViewBag.TenantName = subscription.TenantName;

                return View();
            }
        }
    }
}