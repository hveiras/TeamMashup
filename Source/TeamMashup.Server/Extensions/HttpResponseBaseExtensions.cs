using System.Net;
using TeamMashup.Server;

namespace System.Web.Mvc
{
    public static class HttpResponseBaseExtensions
    {
        public static void SetStatusCode(this HttpResponseBase response, HttpStatusCode code)
        {
            SetStatusCode(response, (int)code);
        }

        public static void SetStatusCode(this HttpResponseBase response, HttpStatusCodeExtended code)
        {
            SetStatusCode(response, (int)code);
        }

        private static void SetStatusCode(HttpResponseBase response, int code)
        {
            response.Clear();
            response.TrySkipIisCustomErrors = true;
            response.StatusCode = code;
        }
    }
}