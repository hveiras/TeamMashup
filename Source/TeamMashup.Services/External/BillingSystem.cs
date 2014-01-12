using System;
using TeamMashup.Core.Domain;

namespace TeamMashup.Billing.External
{
    public class BillingSystem
    {
        public bool TryAuthenticate(Guid id, string password, out string token)
        {
            token = string.Empty;
            try
            {
                //TODO: Implement a mock code.
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool TryEmitBill(Bill bill, out string billingCode)
        {
            billingCode = string.Empty;
            try
            {
                //TODO: Implement a mock code.
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}