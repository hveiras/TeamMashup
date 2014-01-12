using System.Web.Mvc;
using TeamMashup.Server.Filters;

namespace TeamMashup.Server
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new CustomHandleErrorAttribute());
        }
    }
}