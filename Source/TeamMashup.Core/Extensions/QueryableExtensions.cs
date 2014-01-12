using System.Linq;
using TeamMashup.Core.Domain;

namespace TeamMashup.Core.Extensions
{
    public static class QueryableExtensions
    {
        public const int MaxPageSize = 1000;
        public const int MinPage = 1;

        public static IQueryable<T> Paged<T>(this IQueryable<T> source, int page, int pageSize) where T : Entity
        {
            if (page < MinPage)
                page = MinPage;

            if (pageSize > MaxPageSize)
                pageSize = MaxPageSize;

            return source.Skip((page - 1) * pageSize).Take(pageSize);
        }
    }
}