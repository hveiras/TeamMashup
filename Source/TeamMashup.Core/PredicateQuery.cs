using System;
using System.Linq;
using System.Linq.Expressions;

namespace TeamMashup.Core
{
    public class PredicateQuery<T> where T : class
    {
        public IQueryable<T> Query { get; private set; }

        public Expression<Func<T, bool>> Predicate { get; set; }

        public PredicateQuery(IQueryable<T> query, Expression<Func<T, bool>> predicate)
        {
            Query = query;
            Predicate = predicate;
        }
    }
}
