using System;
using System.Collections.Generic;
using System.Linq;
using TeamMashup.Core.Enums;
using TeamMashup.Core.Extensions;

namespace TeamMashup.Core.Domain.Services
{
    public class LogService : IService
    {
        public IDatabaseContext Database { get; private set; }

        public long SubscriptionId { get; private set; }

        public LogService(long subscriptionId, IDatabaseContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            Database = context;
            SubscriptionId = subscriptionId;
        }

        public IEnumerable<Log> GetLogs(LogEntryLevel level, DateTime? fromDate = null, DateTime? toDate = null, int page = 1, int pageSize = 25)
        {
            var query = Database.Logs;

            if (level != LogEntryLevel.All)
                query.Where(x => x.Level == level);

            if (fromDate.HasValue)
                query.Where(x => x.DateTime >= fromDate.Value);

            if (toDate.HasValue)
                query.Where(x => x.DateTime <= toDate.Value);

            return query.Paged<Log>(page, pageSize);
        }
    }
}