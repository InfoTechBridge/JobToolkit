
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Dapper;
using JobToolkit.Core;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;

namespace JobToolkit.Repository.Oracle
{
    public class OracleJobRepository : IJobRepository, IEnumerable
    {
        IFormatter Formatter;
        string ConnectionString;
        
        public OracleJobRepository(string connectionString)
        {
            ConnectionString = connectionString;
            this.Formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
        }

        public OracleJobRepository(string connectionString, IFormatter formatter)
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
                Enabled = job.Enabled ? 1 : 0,
                Status = (short)job.Status,
                ScheduleTime = job.ScheduleTime.ToOracleTimeStampTZ(),
                Cron = job.Cron?.Expression,
                NextScheduleTime = job.NextScheduleTime.ToOracleTimeStampTZ(),
                ScheduleEndTime = job.ScheduleEndTime.ToOracleTimeStampTZ(),
                LastExecutanInfoId = job.LastExecutanInfo?.Id,
                MaxRetryAttempts = job.AutomaticRetry?.MaxRetryAttempts,
                Owner = job.Owner,
                Description = job.Description,
                CreateTime = job.CreateTime.ToOracleTimeStampTZ(),
                UpdateTime = job.UpdateTime.ToOracleTimeStampTZ(),
            };

            string sql = @"
                insert into job(Id, Title, TaskData, Priority, Enabled, Status, ScheduleTime, Cron, NextScheduleTime, ScheduleEndTime, LastExecutanInfoId, MaxRetryAttempts, Owner, Description, CreateTime, UpdateTime)
                values(:Id, :Title, :TaskData, :Priority, :Enabled, :Status, :ScheduleTime, :Cron, :NextScheduleTime, :ScheduleEndTime, :LastExecutanInfoId, :MaxRetryAttempts, :Owner, :Description, :CreateTime, :UpdateTime)
            ";

            var parameters = new OracleDynamicParameters();
            parameters.Add("Id", OracleDbType.Varchar2, param.Id);
            parameters.Add("Title", OracleDbType.NVarchar2, param.Title);
            parameters.Add("TaskData", OracleDbType.NClob, param.TaskData);
            parameters.Add("Priority", OracleDbType.Int16, param.Priority);
            parameters.Add("Enabled", OracleDbType.Int16, param.Enabled);
            parameters.Add("Status", OracleDbType.Int16, param.Status);
            parameters.Add("ScheduleTime", OracleDbType.TimeStampTZ, param.ScheduleTime);
            parameters.Add("Cron", OracleDbType.Varchar2, param.Cron);
            parameters.Add("NextScheduleTime", OracleDbType.TimeStampTZ, param.NextScheduleTime);
            parameters.Add("ScheduleEndTime", OracleDbType.TimeStampTZ, param.ScheduleEndTime);
            parameters.Add("LastExecutanInfoId", OracleDbType.TimeStampTZ, param.LastExecutanInfoId);
            parameters.Add("MaxRetryAttempts", OracleDbType.Int32, param.MaxRetryAttempts);
            parameters.Add("Owner", OracleDbType.NVarchar2, param.Owner);
            parameters.Add("Description", OracleDbType.NVarchar2, param.Description);
            parameters.Add("CreateTime", OracleDbType.TimeStampTZ, param.CreateTime);
            parameters.Add("UpdateTime", OracleDbType.TimeStampTZ, param.UpdateTime);

            using (OracleConnection conn = new OracleConnection(ConnectionString))
            {
                conn.Open();
                                
                conn.Execute(sql, parameters);

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
                Enabled = job.Enabled ? 1 : 0,
                Status = (short)job.Status,
                ScheduleTime = job.ScheduleTime.ToOracleTimeStampTZ(),
                Cron = job.Cron?.Expression,
                NextScheduleTime = job.NextScheduleTime.ToOracleTimeStampTZ(),
                ScheduleEndTime = job.ScheduleEndTime.ToOracleTimeStampTZ(),
                LastExecutanInfoId = job.LastExecutanInfo?.Id,
                MaxRetryAttempts = job.AutomaticRetry?.MaxRetryAttempts,
                Owner = job.Owner,
                Description = job.Description,
                UpdateTime = job.UpdateTime.ToOracleTimeStampTZ(),
            };

            string sql = @"
                update job set
                Title = :Title, TaskData = :TaskData, Priority = :Priority, Enabled = :Enabled, Status = : Status
                , ScheduleTime = :ScheduleTime, Cron = :Cron, NextScheduleTime = :NextScheduleTime, ScheduleEndTime = :ScheduleEndTime,
                , LastExecutanInfoId = :LastExecutanInfoId, MaxRetryAttempts = :MaxRetryAttempts, Owner = :Owner, Description = :Description
                , UpdateTime = :UpdateTime
                where id = :Id;
            ";

            var parameters = new OracleDynamicParameters();
            parameters.Add(":Id", OracleDbType.Varchar2, param.Id);
            parameters.Add(":Title", OracleDbType.NVarchar2, param.Title);
            parameters.Add(":TaskData", OracleDbType.NClob, param.TaskData);
            parameters.Add(":Priority", OracleDbType.Int16, param.Priority);
            parameters.Add(":Enabled", OracleDbType.Int16, param.Enabled);
            parameters.Add(":Status", OracleDbType.Int16, param.Status);
            parameters.Add(":ScheduleTime", OracleDbType.TimeStampTZ, param.ScheduleTime);
            parameters.Add(":Cron", OracleDbType.Varchar2, param.Cron);
            parameters.Add(":NextScheduleTime", OracleDbType.TimeStampTZ, param.NextScheduleTime);
            parameters.Add(":ScheduleEndTime", OracleDbType.TimeStampTZ, param.ScheduleEndTime);
            parameters.Add(":LastExecutanInfoId", OracleDbType.Varchar2, param.LastExecutanInfoId);
            parameters.Add(":MaxRetryAttempts", OracleDbType.Int32, param.MaxRetryAttempts);
            parameters.Add(":Owner", OracleDbType.NVarchar2, param.Owner);
            parameters.Add(":Description", OracleDbType.NVarchar2, param.Description);
            //parameters.Add(":CreateTime", OracleDbType.TimeStampTZ, param.CreateTime);
            parameters.Add(":UpdateTime", OracleDbType.TimeStampTZ, param.UpdateTime);

            using (OracleConnection conn = new OracleConnection(ConnectionString))
            {
                conn.Open();

                conn.Execute(sql, parameters);

                conn.Close();
            }

        }
        public void UpdatExecStatus(Job job)
        {
            var param = new
            {
                Id = job.Id,
                Status = (short)job.Status,
                NextScheduleTime = job.NextScheduleTime.ToOracleTimeStampTZ(),
                LastExecutanInfoId = job.LastExecutanInfo?.Id,
                UpdateTime = job.UpdateTime.ToOracleTimeStampTZ(),
                // Last Executan Info
                RetryNumber = job.LastExecutanInfo?.RetryNumber,
                StartTime = job.LastExecutanInfo?.StartTime.ToOracleTimeStampTZ(),
                EndTime = job.LastExecutanInfo?.EndTime.ToOracleTimeStampTZ(),
                ExecStatus = (short)job.LastExecutanInfo?.Status,
                ExecDescription = job.LastExecutanInfo?.Description,
                NextRetryTime = job.LastExecutanInfo?.NextRetryTime.ToOracleTimeStampTZ(),
            };
            
            string sql = @"BEGIN
                update job set
                Status = :Status, NextScheduleTime = :NextScheduleTime, LastExecutanInfoId = :LastExecutanInfoId, UpdateTime = :UpdateTime
                where id = :Id;
            ";

            var parameters = new OracleDynamicParameters();

            parameters.Add(":Status", OracleDbType.Int16, param.Status);
            parameters.Add(":NextScheduleTime", OracleDbType.TimeStampTZ, param.NextScheduleTime);
            parameters.Add(":LastExecutanInfoId", OracleDbType.Varchar2, param.LastExecutanInfoId);
            parameters.Add(":UpdateTime", OracleDbType.TimeStampTZ, param.UpdateTime);
            parameters.Add(":Id", OracleDbType.Varchar2, param.Id);

            if (job.LastExecutanInfo != null)
            {
                if (job.LastExecutanInfo.Status == JobExecutanStatus.Running) // new execInfo
                {
                    sql += @"
                        insert into JobExec(Id, JobId, RetryNumber, StartTime, EndTime, Status, Description, NextRetryTime)
                        values(:LastExecutanInfoId, :Id, :RetryNumber, :StartTime, :EndTime, :ExecStatus, :ExecDescription, :NextRetryTime);
                    ";

                    parameters.Add(":LastExecutanInfoId", OracleDbType.Varchar2, param.LastExecutanInfoId);
                    parameters.Add(":RetryNumber", OracleDbType.Int32, param.RetryNumber);
                    parameters.Add(":StartTime", OracleDbType.TimeStampTZ, param.StartTime);
                    parameters.Add(":EndTime", OracleDbType.TimeStampTZ, param.EndTime);
                    parameters.Add(":ExecStatus", OracleDbType.Int16, param.ExecStatus);
                    parameters.Add(":ExecDescription", OracleDbType.NVarchar2, param.ExecDescription);
                    parameters.Add(":NextRetryTime", OracleDbType.TimeStampTZ, param.NextRetryTime);
                }
                else
                {
                    sql += @"
                        update JobExec set 
                        EndTime = :EndTime, Status = :ExecStatus, Description = :ExecDescription, NextRetryTime = :NextRetryTime
                        where Id = :LastExecutanInfoId;
                    ";

                    parameters.Add(":EndTime", OracleDbType.TimeStampTZ, param.EndTime);
                    parameters.Add(":ExecStatus", OracleDbType.Int16, param.ExecStatus);
                    parameters.Add(":ExecDescription", OracleDbType.NVarchar2, param.ExecDescription);
                    parameters.Add(":NextRetryTime", OracleDbType.TimeStampTZ, param.NextRetryTime);
                    parameters.Add(":LastExecutanInfoId", OracleDbType.Varchar2, param.LastExecutanInfoId);
                }
            }

            sql += "\nEND;";

            using (OracleConnection conn = new OracleConnection(ConnectionString))
            {
                conn.Open();

                conn.Execute(sql, parameters);

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
                select Job.*, JobExec.RetryNumber, JobExec.StartTime, JobExec.EndTime
                       , JobExec.Status as ExecStatus, JobExec.Description as ExecDescription, JobExec.NextRetryTime
                from job left join JobExec on job.LastExecutanInfoId = JobExec.id
            ";

            var parameters = new OracleDynamicParameters();
            string whereStr = ParseCriteria(criteria, parameters);
            if (!string.IsNullOrEmpty(whereStr))
                sql += " where " + whereStr;

            List<Job> jobs = new List<Job>();
            using (OracleConnection conn = new OracleConnection(ConnectionString))
            {
                conn.Open();
                                              
                var resault = conn.Query(sql, parameters); //conn.Query(sql, criteria);

                foreach (var item in resault)
                {
                    JobExecutanInfo lastExecutanInfo = null;
                    if (item.LASTEXECUTANINFOID != null)
                    {
                        lastExecutanInfo = new JobExecutanInfo()
                        {
                            Id = item.LASTEXECUTANINFOID,
                            JobId = item.ID,
                            RetryNumber = item.RETRYNUMBER,
                            StartTime = item.STARTTIME,
                            EndTime = item.ENDTIME,
                            Status = (JobExecutanStatus)item.EXECSTATUS,
                            Description = item.EXECDESCRIPTION,
                            NextRetryTime = item.NEXTRETRYTIME,
                        };
                    }

                    Job job = new Job(Deserialize(item.TASKDATA))
                    {
                        Id = item.ID,
                        Title = item.TITLE,
                        Priority = item.PRIORITY,
                        Enabled = ((short)item.ENABLED) > 0 ? true : false,
                        Status = (JobStatus)item.STATUS,
                        Cron = item.CRON != null ? new CronExpression(item.CRON) : null,
                        ScheduleTime = item.SCHEDULETIME,
                        NextScheduleTime = item.NEXTSCHEDULETIME,
                        ScheduleEndTime = item.SCHEDULEENDTIME,
                        LastExecutanInfo = lastExecutanInfo,
                        AutomaticRetry = item.MAXRETRYATTEMPTS != null ? new AutomaticRetryPolicy() { MaxRetryAttempts = item.MAXRETRYATTEMPTS } : null,
                        Owner = item.OWNER,
                        Description = item.DESCRIPTION,
                        CreateTime = item.CREATETIME,
                        UpdateTime = item.UPDATETIME,
                    };

                    jobs.Add(job);
                }
                conn.Close();
            }
          
            return jobs;
        }
        private string ParseCriteria(JobDataQueryCriteria criteria, OracleDynamicParameters parameters)
        {
            
            //OracleParameterCollection parameters
            string whereStr = "";

            if (criteria.Id != null)
            {
                whereStr += " and job.Id = :Id";
                //parameters.Add("Id", OracleDbType.Varchar2, criteria.Id, System.Data.ParameterDirection.Input);
                parameters.Add(":Id", OracleDbType.Varchar2, criteria.Id);
            }
            if (criteria.NextScheduleTimeIsNull != null)
            {
                if (criteria.NextScheduleTimeIsNull.GetValueOrDefault())
                    whereStr += " and job.NextScheduleTime is null";

                else
                    whereStr += " and job.NextScheduleTime is not null";
            }

            if (criteria.NextScheduleTimeBe != null)
            {
                whereStr += " and job.NextScheduleTime >= :NextScheduleTimeBe";
                //parameters.Add("NextScheduleTimeBe", OracleDbType.TimeStampTZ, criteria.NextScheduleTimeBe, System.Data.ParameterDirection.Input);
                parameters.Add(":NextScheduleTimeBe", OracleDbType.TimeStampTZ, criteria.NextScheduleTimeBe.ToOracleTimeStampTZ());
            }

            if (criteria.NextScheduleTimeLe != null)
            {
                whereStr += " and job.NextScheduleTime <= :NextScheduleTimeLe";
                //parameters.Add("NextScheduleTimeLe", OracleDbType.TimeStampTZ, criteria.NextScheduleTimeLe, System.Data.ParameterDirection.Input);
                parameters.Add(":NextScheduleTimeLe", OracleDbType.TimeStampTZ, criteria.NextScheduleTimeLe.ToOracleTimeStampTZ());
            }
            if (criteria.Status != null)
            {
                whereStr += " and job.Status = :Status";
                //parameters.Add("Status", OracleDbType.Int16, (short)criteria.Status, System.Data.ParameterDirection.Input);
                parameters.Add(":Status", OracleDbType.Int16, (short)criteria.Status);
            }
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

        private byte[] SerializeTask(JobTask task)
        {
            byte[] taskData;
            using (MemoryStream stream = new MemoryStream())
            {
                Formatter.Serialize(stream, task);
                taskData = stream.ToArray();
            }
            return taskData;
        }
        private string SerializeTaskAsString(JobTask task)
        {
            byte[] taskData = SerializeTask(task);
            //return Encoding.UTF8.GetString(taskData);
            return BitConverter.ToString(taskData).Replace("-", string.Empty);
        }
        private JobTask Deserialize(byte[] taskData)
        {
            JobTask task;
            using (MemoryStream stream = new MemoryStream(taskData))
            {
                task = (JobTask)Formatter.Deserialize(stream);
            }
            return task;
        }
        private JobTask Deserialize(string taskDataStr)
        {
            //byte[] taskData = Encoding.UTF8.GetBytes(taskDataStr);
            byte[] taskData = StringToByteArray(taskDataStr);
            return Deserialize(taskData);
        }
        private static byte[] StringToByteArray(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }
    }
}
