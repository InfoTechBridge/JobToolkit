using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace JobToolkit.Core
{
    public static class ExpressionTaskExtension
    {
        public static Job Schedule(this JobManager jobManager, Expression<Action> expression, AutomaticRetryPolicy automaticRetry = null)
        {
            return jobManager.Schedule(new ExpressionTask(expression), automaticRetry);
        }
        public static Job Schedule(this JobManager jobManager, Expression<Action> expression, DateTimeOffset scheduleTime, AutomaticRetryPolicy automaticRetry = null)
        {
            return jobManager.Schedule(new ExpressionTask(expression), scheduleTime, automaticRetry);
        }
        public static Job Schedule(this JobManager jobManager, Expression<Action> expression, TimeSpan offset, AutomaticRetryPolicy automaticRetry = null)
        {
            return jobManager.Schedule(new ExpressionTask(expression), offset, automaticRetry);
        }
        public static Job Schedule(this JobManager jobManager, Expression<Action> expression, CronExpression cron, DateTimeOffset? scheduleEndTime = null, AutomaticRetryPolicy automaticRetry = null)
        {
            return jobManager.Schedule(new ExpressionTask(expression), cron, scheduleEndTime, automaticRetry);
        }
        public static Job Schedule(this JobManager jobManager, Expression<Action> expression, DateTimeOffset scheduleTime, CronExpression cron, DateTimeOffset? scheduleEndTime = null, AutomaticRetryPolicy automaticRetry = null)
        {
            return jobManager.Schedule(new ExpressionTask(expression), scheduleTime, cron, scheduleEndTime, automaticRetry);
        }
        
    }
}
