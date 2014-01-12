using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using TeamMashup.Core.Enums;

namespace TeamMashup.Core.Domain
{
    public class Subscription : Entity
    {
        public long? SubscriptorId { get; set; }

        public virtual User Subscriptor { get; set; }

        public virtual ICollection<User> Administrators { get; set; }

        public string CompanyName { get; set; }

        public string TenantName { get; set; }

        public string Email { get; set; }

        public string EmailDomain { get; set; }

        public long SubscriptionPlanId { get; set; }

        public virtual SubscriptionPlan SubscriptionPlan { get; set; }

        public string BillingAddress { get; set; }

        public string CompanyAddress { get; set; }

        public long CountryId { get; set; }

        public virtual Country Country { get; set; }

        public string CreditCardExpireDate { get; set; }

        public string CreditCardNumber { get; set; }

        internal int StateValue { get; set; }

        public DateTime? CurrentPeriodStartDate { get; set; }

        public DateTime? CurrentPeriodEndDate { get; set; }

        public string SecurityCode { get; set; }

        public bool HasPendingBills { get; set; }

        public Guid? ActivationConfirmCode { get; set; } //TODO: when activation is implemented, this property should be required.

        public DateTime? CancelDate { get; set; }

        public SubscriptionState State
        {
            get { return (SubscriptionState)StateValue; }
            set { StateValue = (int)value; }
        }

        public Subscription() { }

        public Subscription(string companyName, string tenantName, string email, string emailDomain, long planId, long countryId, string address) : this()
        {
            CompanyName = companyName;
            TenantName = tenantName;
            Email = email;
            EmailDomain = emailDomain;
            SubscriptionPlanId = planId;
            CountryId = countryId;
            CompanyAddress = address;
            State = SubscriptionState.ActivationPending;
        }

        public void Cancel()
        {
            CancelDate = DateTime.UtcNow;
            State = SubscriptionState.Cancelled;
        }

        public void Reactivate()
        {
            if (State != SubscriptionState.Cancelled)
                return;

            State = SubscriptionState.Active;
            CancelDate = null;
        }

        public void SetNextState()
        {
            if (State != SubscriptionState.Cancelled)
            {
                StateValue++;
            }
        }
    }

    public class SubscriptionConfiguration : EntityTypeConfiguration<Subscription>
    {
        public SubscriptionConfiguration()
        {
            Ignore(x => x.State);

            Property(x => x.CompanyName)
                .IsRequired()
                .HasMaxLength(100);

            Property(x => x.TenantName)
                .IsRequired()
                .HasMaxLength(25);

            Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(255);

            Property(x => x.EmailDomain)
                .IsRequired()
                .HasMaxLength(50);

            Property(x => x.CompanyAddress)
                .HasMaxLength(100);

            Property(x => x.BillingAddress)
                .HasMaxLength(100);

            Property(x => x.CreditCardExpireDate)
                .HasMaxLength(10);

            Property(x => x.CreditCardNumber)
                .HasMaxLength(100);

            Property(x => x.StateValue)
                .HasColumnName("State")
                .IsRequired();

            Property(x => x.SecurityCode)
                .HasMaxLength(10);
        }
    }
}