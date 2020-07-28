using JobToolkit.Core;
using ORMToolkit.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobToolkit.DBRepository
{
    [TableInfo(tableName: "JobExec")]
    public class DBJobExec
    {
        [PrimaryKey]
        public string Id { get; set; }
        public string JobId { get; set; }
        public int RetryNumber { get; set; }

        [Mapper(typeof(SqlLiteDateTimeOffsetMapper))]
        public DateTimeOffset StartTime { get; set; }

        [Mapper(typeof(SqlLiteDateTimeOffsetMapper))]
        public DateTimeOffset? EndTime { get; set; }

        public JobExecutanStatus Status { get; set; }
        public string Description { get; set; }

        [Mapper(typeof(SqlLiteDateTimeOffsetMapper))]
        public DateTimeOffset? NextRetryTime { get; set; }
    }
}
