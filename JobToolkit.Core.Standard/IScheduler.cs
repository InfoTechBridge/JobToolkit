using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobToolkit.Core
{
    public interface IScheduler
    {
        /// <summary>
        /// Add job to queue
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        Job Enqueue(Job job);

        /// <summary>
        /// remove job from queue
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        Job Dequeue(string jobId);

        /// <summary>
        /// Immidiate run
        /// </summary>
        /// <param name="task"></param>
        /// <param name="retryPolicy"></param>
        /// <returns></returns>
        Job Schedule(JobTask task, AutomaticRetryPolicy automaticRetry = null);

        /// <summary>
        /// Run at specific time
        /// </summary>
        /// <param name="task"></param>
        /// <param name="scheduleTime"></param>
        /// <param name="retryPolicy"></param>
        /// <returns></returns>
        Job Schedule(JobTask task, DateTimeOffset scheduleTime, AutomaticRetryPolicy automaticRetry = null);

        /// <summary>
        /// Run after offcet time
        /// </summary>
        /// <param name="task"></param>
        /// <param name="offset"></param>
        /// <param name="automaticRetry"></param>
        /// <returns></returns>
        Job Schedule(JobTask task, TimeSpan offset, AutomaticRetryPolicy automaticRetry = null);

        /// <summary>
        /// Schedule 
        /// </summary>
        /// <param name="task"></param>
        /// <param name="scheduleTime"></param>
        /// <param name="cron"></param>
        /// <param name="scheduleEndTime"></param>
        /// <param name="automaticRetry"></param>
        /// <returns></returns>
        Job Schedule(JobTask task, DateTimeOffset scheduleTime, CronExpression cron, DateTimeOffset? scheduleEndTime, AutomaticRetryPolicy automaticRetry);
    }
}
