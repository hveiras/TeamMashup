using System;
using TeamMashup.Core.Domain;
using TeamMashup.Core.Enums;

namespace TeamMashup.Core.Tracking
{
    public class LogHelper
    {
        public static void Info(string message, long subscriptionId = Constants.InvalidId, IDatabaseContext context = null)
        {
            Log(message, LogEntryLevel.Information, subscriptionId, context);
        }

        public static void Info(Exception exception, long subscriptionId = Constants.InvalidId, IDatabaseContext context = null)
        {
            Log(exception, LogEntryLevel.Information, subscriptionId, context);
        }

        public static void Warining(string message, long subscriptionId = Constants.InvalidId, IDatabaseContext context = null)
        {
            Log(message, LogEntryLevel.Warning, subscriptionId, context);
        }

        public static void Warining(Exception exception, long subscriptionId = Constants.InvalidId, IDatabaseContext context = null)
        {
            Log(exception, LogEntryLevel.Warning, subscriptionId, context);
        }

        public static void Error(string message, long subscriptionId = Constants.InvalidId, IDatabaseContext context = null)
        {
            Log(message, LogEntryLevel.Error, subscriptionId, context);
        }

        public static void Error(Exception exception, long subscriptionId = Constants.InvalidId, IDatabaseContext context = null)
        {
            Log(exception, LogEntryLevel.Error, subscriptionId, context);
        }

        public static void Critical(string message, long subscriptionId = Constants.InvalidId, IDatabaseContext context = null)
        {
            Log(message, LogEntryLevel.Critical, subscriptionId, context);
        }

        public static void Critical(Exception exception, long subscriptionId = Constants.InvalidId, IDatabaseContext context = null)
        {
            Log(exception, LogEntryLevel.Critical, subscriptionId, context);
        }

        private static void Log(string message, LogEntryLevel level, long subscriptionId, IDatabaseContext context = null)
        {
            var log = new Log 
            { 
                SubscriptionId = subscriptionId,
                Message = message,
                Level = level 
            };

            if (context != null)
            {
                Log(log, context);
            }
            else
            {
                using (var newContext = new DatabaseContext())
                {
                    Log(log, newContext);
                }
            }
        }

        private static void Log(Exception exception, LogEntryLevel level, long subscriptionId, IDatabaseContext context = null)
        {
            var log = new Log
            {
                SubscriptionId = subscriptionId,
                Message = exception.Message,
                Level = level,
                Source = exception.Source,
                StackTrace = exception.StackTrace
            };

            if (context != null)
            {
                Log(log, context);
            }
            else
            {
                using (var newContext = new DatabaseContext())
                {
                    Log(log, newContext);
                }
            }
        }

        private static void Log(Log log, IDatabaseContext context)
        {
            SafeLog(() =>
            {
                context.Logs.Add(log);
                context.SaveChanges();
            });
        }

        private static void SafeLog(Action log)
        {
            try
            {
                log();
            }
            catch (Exception ex)
            {
                throw;
                //TODO: log in text file.
            }
        }
    }
}
