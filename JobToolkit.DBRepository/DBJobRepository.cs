using JobToolkit.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Dapper;
using System.Data.SqlClient;
using System.Data.Common;
using Serialize.Linq.Serializers;

namespace JobToolkit.DBRepository
{
    public class DBJobRepository : IJobRepository, IEnumerable
    {
        IFormatter Formatter;
        string ConnectionString;
        
        public DBJobRepository()
        {
            this.Formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            ConnectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=JobToolkit;Integrated Security=True";
        }

        public DBJobRepository(string connectionString, IFormatter formatter)
        {
            ConnectionString = connectionString;
            this.Formatter = formatter;
        }
        
        public string Add(Job job)
        {
            var param = new
            {
                Id = job.Id,
                Title = job.Title,
                TaskData = SerializeTaskAsString(job.Task),
                Priority = job.Priority,
                Enabled = job.Enabled,
                Status = (short)job.Status,
                ScheduleTime = job.ScheduleTime,
                Cron = job.Cron?.Expression,
                NextScheduleTime = job.NextScheduleTime,
                ScheduleEndTime = job.ScheduleEndTime,
                LastExecutanInfoId = job.LastExecutanInfo?.Id,
                MaxRetryAttempts = job.AutomaticRetry?.MaxRetryAttempts,
                Owner = job.Owner,
                Description = job.Description,
                CreateTime = job.CreateTime,
                UpdateTime = job.UpdateTime,
            };

            string sql = @"
                insert into job(Id, Title, TaskData, Priority, Enabled, Status, ScheduleTime, Cron, NextScheduleTime, ScheduleEndTime, LastExecutanInfoId, MaxRetryAttempts, Owner, Description, CreateTime, UpdateTime)
                values(@Id, @Title, @TaskData, @Priority, @Enabled, @Status, @ScheduleTime, @Cron, @NextScheduleTime, @ScheduleEndTime, @LastExecutanInfoId, @MaxRetryAttempts, @Owner, @Description, @CreateTime, @UpdateTime)
            ";
    	
            using (DbConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                conn.Execute(sql, param);

                conn.Close();
            }

            return job.Id;
        }
        public Job Get(string jobId)
        {
            return GetAll(new JobDataQueryCriteria() { Id = jobId }).FirstOrDefault();
        }
        public void Update(Job job)
        {
            var param = new
            {
                Id = job.Id,
                Title = job.Title,
                TaskData = SerializeTaskAsString(job.Task),
                Priority = job.Priority,
                Enabled = job.Enabled,
                Status = (short)job.Status,
                ScheduleTime = job.ScheduleTime,
                Cron = job.Cron?.Expression,
                NextScheduleTime = job.NextScheduleTime,
                ScheduleEndTime = job.ScheduleEndTime,
                LastExecutanInfoId = job.LastExecutanInfo?.Id,
                MaxRetryAttempts = job.AutomaticRetry?.MaxRetryAttempts,
                Owner = job.Owner,
                Description = job.Description,
                UpdateTime = job.UpdateTime,
            };

            string sql = @"
                update job set
                Title = @Title, TaskData = @TaskData, Priority = @Priority, Enabled = @Enabled, Status = @ Status
                , ScheduleTime = @ScheduleTime, Cron = @Cron, NextScheduleTime = @NextScheduleTime, ScheduleEndTime = @ScheduleEndTime,
                , LastExecutanInfoId = @LastExecutanInfoId, MaxRetryAttempts = @MaxRetryAttempts, Owner = @Owner, Description = @Description
                , UpdateTime = @UpdateTime
                where id = @Id;
            ";

            using (DbConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                conn.Execute(sql, param);

                conn.Close();
            }

        }
        public void UpdatExecStatus(Job job)
        {
            var param = new
            {
                Id = job.Id,
                Status = (short)job.Status,
                NextScheduleTime = job.NextScheduleTime,
                LastExecutanInfoId = job.LastExecutanInfo?.Id,
                UpdateTime = job.UpdateTime,
                // Last Executan Info
                RetryNumber = job.LastExecutanInfo?.RetryNumber,
                StartTime = job.LastExecutanInfo?.StartTime,
                EndTime = job.LastExecutanInfo?.EndTime,
                ExecStatus = (short)job.LastExecutanInfo?.Status,
                ExecDescription = job.LastExecutanInfo?.Description,
                NextRetryTime = job.LastExecutanInfo?.NextRetryTime,
            };

            string sql = @"
                update job set
                Status = @Status, NextScheduleTime = @NextScheduleTime, LastExecutanInfoId = @LastExecutanInfoId, UpdateTime = @UpdateTime
                where id = @Id;
            ";

            if (job.LastExecutanInfo != null)
            {
                if (job.LastExecutanInfo.Status == JobExecutanStatus.Running) // new execInfo
                    sql += @"
                        insert into ExecutanInfo(Id, JobId, RetryNumber, StartTime, EndTime, Status, Description, NextRetryTime)
                        values(@LastExecutanInfoId, @Id, @RetryNumber, @StartTime, @EndTime, @ExecStatus, @ExecDescription, @NextRetryTime);
                    ";
                else
                    sql += @"
                        update ExecutanInfo set 
                        EndTime = @EndTime, Status = @ExecStatus, Description = @ExecDescription, NextRetryTime = @NextRetryTime
                        where Id = @LastExecutanInfoId;
                    ";
            }

            using (DbConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                conn.Execute(sql, param);

                conn.Close();
            }
        }
        public Job Remove(string jobId)
        {
            throw new NotImplementedException();
        }
        public List<Job> GetAll()
        {
            return GetAll(new JobDataQueryCriteria());
        }
        public List<Job> GetAll(JobDataQueryCriteria criteria)
        {
            string sql = @"
                select Job.*, ExecutanInfo.RetryNumber, ExecutanInfo.StartTime, ExecutanInfo.EndTime
                       , ExecutanInfo.Status as ExecStatus, ExecutanInfo.Description as ExecDescription, ExecutanInfo.NextRetryTime
                from job left join ExecutanInfo on job.LastExecutanInfoId = ExecutanInfo.id
            ";

            string whereStr = ParseCriteria(criteria);
            if (!string.IsNullOrEmpty(whereStr))
                sql += " where " + whereStr;

            List<Job> jobs = new List<Job>();
            using (DbConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                var resault = conn.Query(sql, criteria);

                foreach (var item in resault)
                {
                    JobExecutanInfo lastExecutanInfo = null;
                    if (item.LastExecutanInfoId != null)
                    {
                        lastExecutanInfo = new JobExecutanInfo()
                        {
                            Id = item.LastExecutanInfoId,
                            JobId = item.Id,
                            RetryNumber = item.RetryNumber,
                            StartTime = item.StartTime,
                            EndTime = item.EndTime,
                            Status = (JobExecutanStatus)item.ExecStatus,
                            Description = item.ExecDescription,
                            NextRetryTime = item.NextRetryTime,
                        };
                    }

                    Job job = new Job(Deserialize(item.TaskData))
                    {
                        Id = item.Id,
                        Title = item.Title,
                        Priority = item.Priority,
                        Enabled = item.Enabled,
                        Status = (JobStatus)item.Status,
                        Cron = item.Cron != null ? new CronExpression(item.Cron) : null,
                        ScheduleTime = item.ScheduleTime,
                        NextScheduleTime = item.NextScheduleTime,
                        ScheduleEndTime = item.ScheduleEndTime,
                        LastExecutanInfo = lastExecutanInfo,
                        AutomaticRetry = item.MaxRetryAttempts != null ? new AutomaticRetryPolicy() { MaxRetryAttempts = item.MaxRetryAttempts } : null,
                        Owner = item.Owner,
                        Description = item.Description,
                        CreateTime = item.CreateTime,
                        UpdateTime = item.UpdateTime,
                    };

                    jobs.Add(job);
                }
                conn.Close();
            }
          
            return jobs;
        }
        private string ParseCriteria(JobDataQueryCriteria criteria)
        {
            string whereStr = "";

            if (criteria.Id != null)
                whereStr += " and job.Id = @Id";

            if (criteria.NextScheduleTimeIsNull != null)
            {
                if (criteria.NextScheduleTimeIsNull.GetValueOrDefault())
                    whereStr += " and job.NextScheduleTime is null";

                else
                    whereStr += " and job.NextScheduleTime is not null";
            }

            if (criteria.NextScheduleTimeBe != null)
                whereStr += " and job.NextScheduleTime >= @NextScheduleTimeBe";

            if (criteria.NextScheduleTimeLe != null)
                whereStr += " and job.NextScheduleTime <= @NextScheduleTimeLe";

            if (criteria.Status != null)
                whereStr += " and job.Status = @Status";

            if (whereStr.StartsWith(" and "))
                whereStr = whereStr.Remove(0, 5);

            return whereStr;
        }

        public IEnumerator<Job> GetEnumerator()
        {
            foreach (var item in GetAll())
                yield return (Job)item;
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            // call the generic version
            return this.GetEnumerator();
        }

        protected byte[] SerializeTask(JobTask task)
        {
            byte[] taskData;
            using (MemoryStream stream = new MemoryStream())
            {
                Formatter.Serialize(stream, task);
                taskData = stream.ToArray();
            }
            return taskData;
        }
        protected string SerializeTaskAsString(JobTask task)
        {
            byte[] taskData = SerializeTask(task);
            //return Encoding.UTF8.GetString(taskData);
            return BitConverter.ToString(taskData).Replace("-", string.Empty);
        }
        protected JobTask Deserialize(byte[] taskData)
        {
            JobTask task;
            using (MemoryStream stream = new MemoryStream(taskData))
            {
                task = (JobTask)Formatter.Deserialize(stream);
            }
            return task;
        }
        protected JobTask Deserialize(string taskDataStr)
        {
            //byte[] taskData = Encoding.UTF8.GetBytes(taskDataStr);
            byte[] taskData = StringToByteArray(taskDataStr);
            return Deserialize(taskData);
        }
        protected static byte[] StringToByteArray(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }
    }
}
