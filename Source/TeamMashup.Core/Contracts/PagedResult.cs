using System.Collections.Generic;

namespace TeamMashup.Core.Contracts
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; }
        
        public int TotalItems { get; set; }

        public PagedResult()
        {
            Items = new List<T>();
        }
    }
}
