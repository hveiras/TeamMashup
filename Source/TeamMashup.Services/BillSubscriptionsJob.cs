using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using TeamMashup.Billing.External;
using TeamMashup.Core.Domain;
using TeamMashup.Core.Enums;
using TeamMashup.Tools.Helpers;

namespace TeamMashup.Services
{
    [DisallowConcurrentExecution]
    public class BillSubscriptionsJob : IJob
    {
        public PaymentSystem Payment { get; private set; }

        public BillingSystem Billing { get; private set; }

        public BillSubscriptionsJob()
        {
            Payment = new PaymentSystem();
            Billing = new BillingSystem();
        }

        public void Execute(IJobExecutionContext jobContext)
        {
            try
            {
                const int MaxSubscriptions = 10; //TODO: move this into a configuration file.
                using (var context = new DatabaseContext())
                {
                    var subscriptions = context.Subscriptions.GetSubscriptionsToBill(MaxSubscriptions).ToList();

                    if (!subscriptions.Any())
                    {
                        Console.WriteLine("Billing Job: No Subscriptions to process");
                        return;
                    }

                    ConsoleHelper.WriteLine(ConsoleColor.Green, string.Format("Billing Job: Processing {0} subscriptions", subscriptions.Count));

                    foreach (var s in subscriptions)
                    {
                        string operationCode;
                        if (Payment.TryAuthenticate(Constants.SystemId, Constants.Password, out operationCode))
                        {
                            if (Payment.TryExecutePayment(s.CreditCardNumber, s.CreditCardExpireDate, s.SecurityCode, s.SubscriptionPlan.Price, operationCode))
                            {
                                ConsoleHelper.WriteLine(ConsoleColor.Green, string.Format("Billing Job: Payment successful for subscription {0}", s.CompanyName));

                                s.CurrentPeriodStartDate = DateTime.UtcNow;
                                s.CurrentPeriodEndDate = DateTime.UtcNow.AddMonths(1);
                                s.State = SubscriptionState.Active;

                                string body = string.Empty;

                                //TODO: Implement this in another way, an excepction in email should not cause the entire payment to fail.
                                //EmailHelper.Send(body, s.Subscriptor.Email, Constants.BillingEmailAddress);

                                s.HasPendingBills = true;

                                string token;
                                if (Billing.TryAuthenticate(Constants.SystemId, Constants.Password, out token))
                                {
                                    ConsoleHelper.WriteLine(ConsoleColor.Green, string.Format("Billing Job: Billed successfuly emited for subscription {0}", s.CompanyName));

                                    var exportType = s.Country.Name.Equals(Core.Constants.LocalCountryName, StringComparison.InvariantCultureIgnoreCase) ? null : "Services";

                                    var bill = new Bill
                                    {
                                        Date = DateTime.UtcNow,
                                        TributaryId = Constants.TributaryId,
                                        Subscription = s,
                                        CustomerAddress = s.CompanyAddress,
                                        CustomerName = s.Subscriptor.Name,
                                        CustomerCountry = s.Country,
                                        ExportType = exportType,
                                        Items = new List<BillItem>
                                        {
                                            new BillItem
                                            {
                                                Description = string.Format("Subscription to TeamMashup, plan {0}", s.SubscriptionPlan.Name),
                                                Price = s.SubscriptionPlan.Price,
                                                Quantity = 1
                                            }
                                        }
                                    };

                                    string billingCode;
                                    if (Billing.TryEmitBill(bill, out billingCode))
                                    {
                                        context.Bills.Add(bill);
                                        s.HasPendingBills = false;

                                        string body2 = string.Empty;

                                        //TODO: Implement this in another way, an excepction in email should not cause the entire payment to fail.
                                        //EmailHelper.Send(body2, s.Subscriptor.Email, Constants.BillingEmailAddress);
                                    }
                                    else //TryEmitBill
                                    {
                                        //TODO: Log error.
                                    }
                                }
                                else //TryAuthenticate
                                {
                                    //TODO: Log error.
                                }
                            }
                            else //TryExecutePayment
                            {
                                s.SetNextState();
                                //TODO: Log error.
                            }
                        }
                        else //TryAuthenticate
                        {
                        }
                    }

                    context.SaveChanges();
                }
            }
            catch (Exception)
            {
                //TODO: Log exeption
            }
        }
    }
}