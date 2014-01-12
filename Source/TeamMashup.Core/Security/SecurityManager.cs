using System;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace TeamMashup.Core.Security
{
    public class SecurityManager
    {
        public static SecureToken GenerateToken()
        {
            return new SecureToken
            {
                Code = Guid.NewGuid(),
                Expires = DateTime.UtcNow.AddHours(1)
            };
        }

        public static bool TryGetToken(string recoveryLink, out Guid token)
        {
            token = Guid.Empty;

            try
            {
                var values = HttpUtility.ParseQueryString(recoveryLink);
                var tokenString = values["token"];

                if (!Guid.TryParse(tokenString, out token))
                    return false;

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string Hash(string input)
        {
            HMACSHA1 hash = new HMACSHA1();
            hash.Key = Encoding.ASCII.GetBytes("2ef07cdc-7b52-457f-8249-f0173f187063");
            return Convert.ToBase64String(hash.ComputeHash(Encoding.Unicode.GetBytes(input)));
        }
    }
}