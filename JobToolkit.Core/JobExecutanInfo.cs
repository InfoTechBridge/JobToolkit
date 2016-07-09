using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobToolkit.Core
{
    [Serializable]
    public class JobExecutanInfo
    {
        public string Id { get; set; }
        public string JobId { get; set; }
        public int RetryNumber { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset? EndTime { get; set; }
        public JobExecutanStatus Status { get; set; }
        public string Description { get; set; }
        public double Duration { get { return (EndTime.GetValueOrDefault(DateTimeOffset.Now) - StartTime).TotalMilliseconds; } }
        public DateTimeOffset? NextRetryTime { get; set; }

        public JobExecutanInfo()
        {
            this.Id = Guid.NewGuid().ToString().Replace("-", string.Empty);
        }
    }
}
