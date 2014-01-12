using System.Collections.Generic;

namespace TeamMashup.Core.Contracts
{
    public class TypeaheadDatum
    {
        public long value { get; set;}

        public string[] tokens { get; set; }

        public string name { get; set; }

        public Dictionary<string, string> data { get; set; }

        public TypeaheadDatum()
        {
            this.tokens = new List<string>().ToArray();
            this.data = new Dictionary<string, string>();
        }
    }
}
