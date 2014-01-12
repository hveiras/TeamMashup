using System.Collections.Generic;

namespace TeamMashup.Core.Contracts
{
    public class DataTablePage
    {
        public string sEcho { get; set; }

        public int iTotalRecords { get; set; }

        public int iTotalDisplayRecords { get; set; }

        public IList<IDictionary<string, string>> aaData { get; set; }

        public DataTablePage()
        {
            aaData = new List<IDictionary<string, string>>();
        }
    }
}