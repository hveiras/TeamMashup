using Microsoft.Web.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using TeamMashup.Core;
using TeamMashup.Core.Contracts;
using TeamMashup.Core.Domain;
using TeamMashup.Models.Internal;
using TeamMashup.Server.Filters;

namespace TeamMashup.Server.Areas.Internal.Controllers
{
    [NoCache]
    public class BillingController : InternalBaseController
    {
        public ActionResult Index()
        {
            return View("Subscriptions");
        }

        public ActionResult GetSubscriptions(int iDisplayStart, int iDisplayLength, string sEcho)
        {

            var query = Context.Subscriptions.OrderByDescending(x => x.TenantName);

            var totalRecords = query.Count();

            var page = query.Skip(iDisplayStart).Take(iDisplayLength).ToList();

            var model = new DataTablePage
            {
                sEcho = sEcho,
                iTotalRecords = totalRecords,
                iTotalDisplayRecords = totalRecords
            };

            var subscriptions = (from x in page
                                 select new SubscriptionsBillingModel
                                 {
                                     Id = x.Id,
                                     TenantName = x.TenantName,
                                     CompanyName = x.CompanyName,
                                     CountryName = x.Country.Name,
                                     PlanName = x.SubscriptionPlan.Name,
                                     State = x.State.ToString()
                                 }).ToList();

            foreach (var subscription in subscriptions)
            {
                var item = new Dictionary<string, string>
                    {
                        {"TenantName", subscription.TenantName},
                        {"CompanyName", subscription.CompanyName},
                        {"CountryName", subscription.CountryName},
                        {"PlanName", subscription.PlanName},
                        {"State", subscription.State},
                        {"HasPendingBills", subscription.HasPendingBills.AsYesNo()},
                        {"DT_RowId", "roleItem_" + subscription.Id}
                    };

                model.aaData.Add(item);
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Details(string tenantName)
        {
            Subscription subscription;
            if (!Context.Subscriptions.TryGetByTenantName(tenantName, out subscription))
                throw new ApplicationException("Subscription with TenantName = " + tenantName + " does not exist");

            var bills = Context.Bills.FilterBySubscription(subscription.Id).ToList();

            var model = new BillingDetailsModel
            {
                TenantName = tenantName,
                SubscriptionId = subscription.Id,
                Bills = (from x in bills
                         select new BillModel
                         {
                             Id = x.Id,
                             CountryName = x.CustomerCountry.Name,
                             CustomerAddress = x.CustomerAddress,
                             CustomerName = x.CustomerName,
                             Date = x.Date,
                             TributaryId = x.TributaryId,
                             Items = (from y in x.Items
                                      select new BillItemModel
                                      {
                                          Price = y.Price,
                                          Description = y.Description,
                                          Quantity = y.Quantity
                                      }).ToList()
                         }).ToList()
            };

            return View(model);
        }


        public ActionResult Bill(long id)
        {
            Bill bill;
            if (!Context.Bills.TryGetById(id, out bill))
                throw new InvalidOperationException("Cannot find bill with Id " + id + " was not found");

            var type = bill.CustomerCountry.Name == Constants.LocalCountryName ? "A" : "E";
            var model = new BillModel
            {
                Id = bill.Id,
                Type = type,
                CountryName = bill.CustomerCountry.Name,
                CustomerAddress = bill.CustomerAddress,
                CustomerName = bill.CustomerName,
                Date = bill.Date,
                TributaryId = bill.TributaryId,
                Items = (from y in bill.Items
                         select new BillItemModel
                         {
                             Price = y.Price,
                             Description = y.Description,
                             Quantity = y.Quantity
                         }).ToList()
            };

            return View(model);
        }

        public ActionResult Reports(long? from, long? to)
        {
            DateTime dateFrom, dateTo;

            if (!from.HasValue)
                dateFrom = DateTime.UtcNow.AddMonths(-1);
            else
                dateFrom = DateTimeExtensions.Epoch.AddSeconds(from.Value);

            if (!to.HasValue)
                dateTo = DateTime.UtcNow;
            else
                dateTo = DateTimeExtensions.Epoch.AddSeconds(to.Value);


            var query = from b in Context.Bills
                        where b.Date >= dateFrom && b.Date <= dateTo
                        orderby b.Date descending
                        select b;

            var bills = (from x in query
                         select new BillingSummaryModel
                         {
                             BillId = x.Id,
                             Date = x.Date,
                             CustomerName = x.CustomerName,
                             TributaryId = x.TributaryId,
                             Total = x.Items.Sum(y => y.Quantity * y.Price)
                         }).ToList();

            decimal totalBilled = 0;
            
            if(Context.Bills.Any())
            {
                totalBilled = (from b in Context.Bills
                               where b.Date >= dateFrom && b.Date <= dateTo
                               select b).Sum(x => x.Items.Select(y => y.Price * y.Quantity).Sum());
            }

            var model = new BillingReportModel
            {
                TotalBilled = totalBilled,
                Bills = bills
            };

            return View(model);
        }

        [ChildActionOnly]
        public ActionResult PeriodFilter()
        {
            return PartialView("_PeriodFilter");
        }

        public ActionResult ExportBilledToExcel(long? from, long? to)
        {
            DateTime dateFrom, dateTo;

            if (!from.HasValue)
                dateFrom = DateTime.UtcNow.AddMonths(-1);
            else
                dateFrom = DateTimeExtensions.Epoch.AddSeconds(from.Value);

            if (!to.HasValue)
                dateTo = DateTime.UtcNow;
            else
                dateTo = DateTimeExtensions.Epoch.AddSeconds(to.Value);


            var query = from b in Context.Bills
                        where b.Date >= dateFrom && b.Date <= dateTo
                        orderby b.Date descending
                        select b;

            var bills = (from x in query
                         select new BillingSummaryModel
                         {
                             BillId = x.Id,
                             Date = x.Date,
                             CustomerName = x.CustomerName,
                             TributaryId = x.TributaryId,
                             Total = x.Items.Sum(y => y.Quantity * y.Price)
                         }).ToList();

            if (bills.Any())
            {
                var gv = new GridView();
                gv.DataSource = bills.ToList();
                gv.DataBind();
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=Billing.xls");
                Response.ContentType = "application/ms-excel";
                Response.Charset = "";
                var sw = new StringWriter();
                var htw = new HtmlTextWriter(sw);
                gv.RenderControl(htw);
                Response.Output.Write(sw.ToString());
                Response.Flush();
                Response.End();
            }

            return this.RedirectToAction<BillingController>(x => x.Reports(from, to));
        }
    }
}
