using ORMToolkit.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobToolkit.Core.Standard.Repositories.Database
{
    public class DBJobSummary : DBJob
    {
        public int RetryNumber { get; set; }

        [Mapper(typeof(SqlLiteDateTimeOffsetMapper))]
        public DateTimeOffset StartTime { get; set; }

        [Mapper(typeof(SqlLiteDateTimeOffsetMapper))]
        public DateTimeOffset? EndTime { get; set; }
        public JobExecutanStatus ExecStatus { get; set; }
        public string ExecDescription { get; set; }

        [Mapper(typeof(SqlLiteDateTimeOffsetMapper))]
        public DateTimeOffset? NextRetryTime { get; set; }
    }
}
