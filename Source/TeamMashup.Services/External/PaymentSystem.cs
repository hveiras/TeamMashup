using System;

namespace TeamMashup.Billing.External
{
    public class PaymentSystem
    {
        public bool TryAuthenticate(Guid id, string password, out string operationCode)
        {
            operationCode = string.Empty;
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

        public bool TryExecutePayment(string creditCardNumber, string expiryDate, string securityCode, decimal price, string operationCode)
        {
            try
            {
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}