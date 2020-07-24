using ORMToolkit.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobToolkit.Core.Standard.Repositories.Database
{
    [TableInfo(tableName: "Job")]
    public class DBJob
    {
        [PrimaryKey]
        public string Id { get; set; }
        public string Title { get; set; }
        public string TaskData { get; set; }
        public int Priority { get; set; }
        public bool Enabled { get; set; }
        public JobStatus Status { get; set; }
        public string Cron { get; set; }
        
        [Mapper(typeof(SqlLiteDateTimeOffsetMapper))]
        public DateTimeOffset ScheduleTime { get; set; }

        [Mapper(typeof(SqlLiteDateTimeOffsetMapper))]
        public DateTimeOffset? NextScheduleTime { get; set; }

        [Mapper(typeof(SqlLiteDateTimeOffsetMapper))]
        public DateTimeOffset? ScheduleEndTime { get; set; }

        public string LastExecutanInfoId { get; set; }
        public int? MaxRetryAttempts { get; set; }
        public string Owner { get; set; }
        public string Description { get; set; }

        [Mapper(typeof(SqlLiteDateTimeOffsetMapper))]
        public DateTimeOffset CreateTime { get; set; }

        [Mapper(typeof(SqlLiteDateTimeOffsetMapper))]
        public DateTimeOffset? UpdateTime { get; set; }
    }
}
