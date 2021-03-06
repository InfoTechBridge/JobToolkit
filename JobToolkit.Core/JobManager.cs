﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using JobToolkit.Core.Configuration;
using System.Diagnostics.Contracts;
using JobToolkit.Core.Factory;

namespace JobToolkit.Core
{
    public class JobManager : IScheduler, IEnumerable<Job>
    {
        IJobRepository Repository;

        private static JobManager defaultManager;

        private JobManager()
            : this(JobToolkitConfiguration.Config.JobManagerConfigurations[JobToolkitConfiguration.Config.DefaultManager])
        {

        }

        public JobManager(IJobRepository repository)
        {
            Repository = repository;
            //JobServer = new JobServer(Repository);
        }

        public JobManager(JobManagerConfiguration configuration)
        {
            var repositoryName = string.IsNullOrEmpty(configuration.Repository.Trim()) ? JobToolkitConfiguration.Config.DefaultRepository : configuration.Repository;
            Repository = JobToolkitFactory.CreateJobRepository(JobToolkitConfiguration.Config.RepositoryConfigurations[repositoryName]);
        }
        
        public static JobManager Default
        {
            get
            {
                if (defaultManager == null)
                    defaultManager = new JobManager();

                return defaultManager;
            }
        }

        public Job Enqueue(Job job)
        {
            job.Id = Repository.Add(job);
            return job;
        }
        public Job Dequeue(string jobId)
        {
            return Repository.Remove(jobId);
        }
        public Job Schedule(JobTask task, AutomaticRetryPolicy automaticRetry = null)
        {
            return Schedule(task, DateTimeOffset.Now, null, null, automaticRetry);
        }
        public Job Schedule(JobTask task, DateTimeOffset scheduleTime, AutomaticRetryPolicy automaticRetry = null)
        {
            return Schedule(task, scheduleTime, null, null, automaticRetry);
        }
        public Job Schedule(JobTask task, TimeSpan offset, AutomaticRetryPolicy automaticRetry = null)
        {
            return Schedule(task, DateTimeOffset.Now.Add(offset), null, null, automaticRetry);
        }
        public Job Schedule(JobTask task, DateTimeOffset scheduleTime, CronExpression cron, DateTimeOffset? scheduleEndTime, AutomaticRetryPolicy automaticRetry)
        {
            Job job = new Job(task, scheduleTime, cron, scheduleEndTime, automaticRetry);
            return Enqueue(job);
        }

        public Job Get(string jobId)
        {
            return Repository.Get(jobId);
        }        
        public IEnumerator<Job> GetEnumerator()
        {
            foreach (Job item in Repository)
                yield return item;
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            // call the generic version
            return this.GetEnumerator();
        }
        public List<Job> GetAll()
        {
            return Repository.GetAll();
        }
        public List<Job> GetAll(JobDataQueryCriteria criteria)
        {
            return Repository.GetAll(criteria);
        }
    }
}
