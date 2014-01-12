
namespace System.Web.Mvc
{
    public static class ViewDataExtensions
    {
        public static string GetBackLink(this ViewDataDictionary viewData)
        {
            return GetAsLink(viewData, "BackLink");
        }

        public static void SetBackLink(this ViewDataDictionary viewData, string url)
        {
            viewData.Add("BackLink", url);
        }

        public static string GetAsLink(this ViewDataDictionary viewData, string key)
        {
            var nolink = "#nowhere";

            if (!viewData.ContainsKey(key))
                return nolink;

            return viewData[key].ToString();
        }
    }
}