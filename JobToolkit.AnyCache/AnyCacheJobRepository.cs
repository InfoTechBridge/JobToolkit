using AnyCache.Core;
using JobToolkit.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace JobToolkit.AnyCache
{
    public class AnyCacheJobRepository : IJobRepository, IEnumerable
    {
        private readonly IAnyCache Cache;

        public AnyCacheJobRepository(IAnyCache cache)
        {
            this.Cache = cache;
        }

        public string Add(Job job)
        {
            string key = Guid.NewGuid().ToString().Replace("-", string.Empty);
            Cache.Add(key, job);
            return key;
        }

        public virtual Job Get(string jobId)
        {
            return Cache.Get<Job>(jobId);
        }

        public virtual List<Job> GetAll()
        {
            List<Job> jobs = new List<Job>();
            foreach (var item in Cache)
                jobs.Add((Job)item.Value);
            return jobs;
        }

        public virtual List<Job> GetAll(JobDataQueryCriteria criteria)
        {
            List<Job> jobs = new List<Job>();
            foreach (var item in Cache)
            {
                if (criteria.IsMatch((Job)item.Value))
                    jobs.Add((Job)item.Value);
            }
            return jobs;
        }

        public virtual IEnumerator<Job> GetEnumerator()
        {
            foreach (var item in Cache)
                yield return (Job)item.Value;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            // call the generic version
            return this.GetEnumerator();
        }

        public virtual Job Remove(string jobId)
        {
            return Cache.Remove<Job>(jobId);
        }

        public virtual void RemoveAll()
        {
            Cache.ClearCache();
        }
        public virtual void Update(Job job)
        {
            Cache.Set(job.Id, job);
        }

        public virtual void UpdatExecStatus(Job job)
        {
            Update(job);

            //// History
            //if (job.LastExecutanInfo != null)
            //{
            //    if (job.LastExecutanInfo.Status == JobExecutanStatus.Running) // new execInfo
            //        ;
            //}
        }


    }
}
