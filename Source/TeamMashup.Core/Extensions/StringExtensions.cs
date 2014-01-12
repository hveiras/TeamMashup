using System;
using System.Text;

namespace TeamMashup.Core
{
    public static class StringExtensions
    {
        public static string RemoveDiacritics(this string input)
        {
            string stFormD = input.Normalize(NormalizationForm.FormD);
            int len = stFormD.Length;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < len; i++)
            {
                System.Globalization.UnicodeCategory uc = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(stFormD[i]);
                if (uc != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(stFormD[i]);
                }
            }
            return (sb.ToString().Normalize(NormalizationForm.FormC));
        }

        public static bool TryParseEmailDomain(this string email, out string domain)
        {
            domain = null;

            try
            {
                var parts = email.Split('@');

                domain = parts[1];
                return true;
            }
            catch
            {    
                return false;
            }
        }

        public static bool TryParseTenantName(this string email, out string companyName)
        {
            companyName = null;
 
            try
            {
                string domain;
                if (!TryParseEmailDomain(email, out domain))
                    return false;

                companyName = domain.Split('.')[0];
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
