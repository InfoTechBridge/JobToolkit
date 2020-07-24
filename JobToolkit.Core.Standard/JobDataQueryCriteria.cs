using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobToolkit.Core
{
    public class JobDataQueryCriteria : IDataQueryCriteria
    {
        public string Id { get; set; }
        public JobStatus? Status { get; set; }
        public bool? NextScheduleTimeIsNull { get; set; }
        public DateTimeOffset? NextScheduleTimeBe { get; set; }
        public DateTimeOffset? NextScheduleTimeLe { get; set; }

        public string SortExpression { get; set; }

        public int? Skip { get; set; }
        public int? Take { get; set; }
        
        public bool IsMatch(Job job)
        {
            bool match = true;

            if (Id != null)
                match = match && (job.Id == Id);

            if (NextScheduleTimeIsNull != null)
            {
                if(NextScheduleTimeIsNull.GetValueOrDefault())
                    match = match && (job.NextScheduleTime == null);

                else
                    match = match && (job.NextScheduleTime != null);
            }

            if (NextScheduleTimeBe != null)
                match = match && (job.NextScheduleTime >= NextScheduleTimeBe);

            if (NextScheduleTimeLe != null)
                match = match && (job.NextScheduleTime <= NextScheduleTimeLe);

            if (Status != null)
                match = match && (job.Status == Status);

            return match;
        }
    }
}
