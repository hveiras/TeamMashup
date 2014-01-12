using System;
using System.Collections.Generic;
using System.Linq;

namespace TeamMashup.Models.Internal
{
    public class SubscriptionsBillingModel
    {
        public long Id { get; set; }

        public string TenantName { get; set; }

        public string CompanyName { get; set; }

        public string PlanName { get; set; }

        public string CountryName { get; set; }

        public string State { get; set; }

        public bool HasPendingBills { get; set; }
    }

    public class BillingDetailsModel
    {
        public long SubscriptionId { get; set; }

        public string TenantName { get; set; }

        public ICollection<BillModel> Bills { get; set; }

        public BillingDetailsModel()
        {
            Bills = new List<BillModel>();
        }
    }

    public class BillModel
    {
        public long Id { get; set; }

        public string Type { get; set; }

        public DateTime Date { get; set; }

        public string TributaryId { get; set; }

        public string CustomerName { get; set; }

        public string CustomerAddress { get; set; }

        public string CountryName { get; set; }

        public ICollection<BillItemModel> Items { get; set; }

        public decimal Price
        {
            get { return Items.Sum(x => x.Quantity * x.Price); }
        }

        public BillModel()
        {
            Items = new List<BillItemModel>();
        }
    }

    public class BillingReportModel
    {
        public decimal TotalBilled { get; set; }

        public ICollection<BillingSummaryModel> Bills { get; set; }

        public BillingReportModel()
        {
            Bills = new List<BillingSummaryModel>();
        }
    }

    public class BillingSummaryModel
    {
        public long BillId { get; set; }

        public DateTime Date { get; set; }

        public string TributaryId { get; set; }

        public string CustomerName { get; set; }

        public decimal Total { get; set; }
    }

    public class BillItemModel
    {
        public string Description { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public decimal Total 
        {
            get { return Quantity * Price; }
        }
    }
}