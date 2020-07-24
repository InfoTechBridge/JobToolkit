using ORMToolkit.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace JobToolkit.Core.Standard.Repositories.Database
{
    public class DBJobRepository : IJobRepository, IEnumerable
    {
        private readonly IFormatter Formatter;
        private readonly IDataManager DataManager;

        public DBJobRepository(IDataManager dataManager)
        {
            this.DataManager = dataManager;
            this.Formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
        }

        public DBJobRepository(IDataManager dataManager, IFormatter formatter)
        {
            this.DataManager = dataManager;
            this.Formatter = formatter;
        }

        public string Add(Job job)
        {
            var dbJob = new DBJob
            {
                Id = job.Id,
                Title = job.Title,
                TaskData = SerializeTaskAsString(job.Task),
                Priority = job.Priority,
                Enabled = job.Enabled,
                Status = job.Status,
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

            DataManager.Insert(dbJob);

            return job.Id;
        }
        public Job Get(string jobId)
        {
            Job job = null;

            var dbJob = DataManager.Get<DBJob>(new { Id = jobId });
            if(dbJob != null)
            {
                job = new Job(Deserialize(dbJob.TaskData))
                {
                    Id = dbJob.Id,
                    Title = dbJob.Title,
                    Priority = dbJob.Priority,
                    Enabled = dbJob.Enabled,
                    Status = dbJob.Status,
                    Cron = dbJob.Cron != null ? new CronExpression(dbJob.Cron) : null,
                    ScheduleTime = dbJob.ScheduleTime,
                    NextScheduleTime = dbJob.NextScheduleTime,
                    ScheduleEndTime = dbJob.ScheduleEndTime,
                    LastExecutanInfo = null,
                    AutomaticRetry = dbJob.MaxRetryAttempts != null ? new AutomaticRetryPolicy() { MaxRetryAttempts = dbJob.MaxRetryAttempts.Value } : null,
                    Owner = dbJob.Owner,
                    Description = dbJob.Description,
                    CreateTime = dbJob.CreateTime,
                    UpdateTime = dbJob.UpdateTime,
                };

                if (dbJob.LastExecutanInfoId != null)
                {
                    var dbJobExec = DataManager.Get<DBJobExec>(new { Id = dbJob.LastExecutanInfoId, JobId = dbJob.Id });
                    if (dbJobExec != null)
                    {
                        job.LastExecutanInfo = new JobExecutanInfo()
                        {
                            Id = dbJobExec.Id,
                            JobId = dbJobExec.JobId,
                            RetryNumber = dbJobExec.RetryNumber,
                            StartTime = dbJobExec.StartTime,
                            EndTime = dbJobExec.EndTime,
                            Status = dbJobExec.Status,
                            Description = dbJobExec.Description,
                            NextRetryTime = dbJobExec.NextRetryTime,
                        };
                    }
                }
            }

            return job;
        }
        public void Update(Job job)
        {
            var dbJob = new
            {
                Id = job.Id,
                Title = job.Title,
                TaskData = SerializeTaskAsString(job.Task),
                Priority = job.Priority,
                Enabled = job.Enabled,
                Status = job.Status,
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

            DataManager.Update<DBJob>(dbJob, new { Id = job.Id });
        }
        public void UpdatExecStatus(Job job)
        {
            var dbJob = new
            {
                Status = job.Status,
                NextScheduleTime = job.NextScheduleTime,
                LastExecutanInfoId = job.LastExecutanInfo?.Id,
                UpdateTime = job.UpdateTime
            };
            
            DataManager.Update<DBJob>(dbJob, new { Id = job.Id });
            
            if (job.LastExecutanInfo != null)
            {
                if (job.LastExecutanInfo.Status == JobExecutanStatus.Running) // new execInfo
                {
                    var dbJobExec = new DBJobExec
                    {
                        Id = job.LastExecutanInfo.Id,
                        JobId = job.LastExecutanInfo.JobId,// job.Id,
                        RetryNumber = job.LastExecutanInfo.RetryNumber,
                        StartTime = job.LastExecutanInfo.StartTime,
                        EndTime = job.LastExecutanInfo.EndTime,
                        Status = job.LastExecutanInfo.Status,
                        Description = job.LastExecutanInfo.Description,
                        NextRetryTime = job.LastExecutanInfo.NextRetryTime,                        
                    };

                    DataManager.Insert(dbJobExec);
                }
                else
                {
                    var dbJobExec = new
                    {
                        EndTime = job.LastExecutanInfo.EndTime,
                        Status = job.LastExecutanInfo.Status,
                        Description = job.LastExecutanInfo.Description,
                        NextRetryTime = job.LastExecutanInfo.NextRetryTime
                    };

                    DataManager.Update<DBJobExec>(dbJobExec, new { Id = job.LastExecutanInfo.Id });
                }
            }
        }
        public Job Remove(string jobId)
        {
            var job = Get(jobId);
            if(job != null)
            {
                //DataManager.Delete<JobExecutanInfo>(new { Id = job.LastExecutanInfo.Id });
                //DataManager.Delete<DBJob>(new { Id = job.Id });
            }
            
            return job;
        }
        public List<Job> GetAll()
        {
            return GetAll(new JobDataQueryCriteria());
        }
        public List<Job> GetAll(JobDataQueryCriteria criteria)
        {
            string sql = @"select Job.*, JobExec.RetryNumber, JobExec.StartTime, JobExec.EndTime
                       , JobExec.Status as ExecStatus, JobExec.Description as ExecDescription, JobExec.NextRetryTime
                from job left join JobExec on job.LastExecutanInfoId = JobExec.id";

            string whereStr = ParseCriteria(criteria);
            if (!string.IsNullOrEmpty(whereStr))
                sql += " where " + whereStr;

            List<Job> jobs = new List<Job>();


            var resault = DataManager.Query<DBJobSummary>(sql, criteria);
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
                    AutomaticRetry = item.MaxRetryAttempts != null ? new AutomaticRetryPolicy() { MaxRetryAttempts = item.MaxRetryAttempts.Value } : null,
                    Owner = item.Owner,
                    Description = item.Description,
                    CreateTime = item.CreateTime,
                    UpdateTime = item.UpdateTime,
                };

                jobs.Add(job);
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
