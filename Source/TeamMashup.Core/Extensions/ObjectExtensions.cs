using System.Dynamic;
using System.Linq;

namespace TeamMashup.Core.Extensions
{
    public static class ObjectExtensions
    {
        public static bool Has(this object obj, string propertyName)
        {
            var dynamic = obj as DynamicObject;
            if (dynamic == null) return false;
            return dynamic.GetDynamicMemberNames().Contains(propertyName);
        }
    }
}
