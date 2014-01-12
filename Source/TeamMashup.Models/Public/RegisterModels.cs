using System.Collections.Generic;
using System.Web.Mvc;

namespace TeamMashup.Models.Public
{
    public class SelectPlanModel
    {
        public ICollection<SubscriptionPlanModel> Plans { get; set; }

        public SelectPlanModel()
        {
            Plans = new List<SubscriptionPlanModel>();
        }
    }

    public class SubscriptionPlanModel
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }

    public class CreateSubscriptionModel
    {
        public long PlanId { get; set; }

        public string PlanName { get; set; }

        public string Email { get; set; }

        public string CompanyName { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }

        public string Name { get; set; }

        public string CompanyAddress { get; set; }

        public string BillingAddress { get; set; }

        public long CountryId { get; set; }

        public SelectList Countries { get; set; }

        public string CreditCardNumber { get; set; }

        public string CreditCardExpireDate { get; set; }

        public string CreditCardSecurityCode { get; set; }
    }
}