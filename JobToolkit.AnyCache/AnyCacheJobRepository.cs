using AnyCache.Core;
using AnyCache.InMemory;
using JobToolkit.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace JobToolkit.AnyCacheRepository
{
    public class AnyCacheJobRepository : IJobRepository, IEnumerable
    {
        private readonly IAnyCache Cache;

        public AnyCacheJobRepository(IAnyCache cache = null)
        {            
            if (cache == null)
                Cache = new InMemoryCache();
            else
                Cache = cache; 
        }

        public string Add(Job job)
        {
            string key = Guid.NewGuid().ToString().Replace("-", string.Empty);
            Cache.Add(key, job);
            return key;
        }

        public Job Get(string jobId)
        {
            return Cache.Get<Job>(jobId);
        }

        public List<Job> GetAll()
        {
            List<Job> jobs = new List<Job>();
            foreach (var item in Cache)
                jobs.Add((Job)item.Value);
            return jobs;
        }

        public List<Job> GetAll(JobDataQueryCriteria criteria)
        {
            List<Job> jobs = new List<Job>();
            foreach (var item in Cache)
            {
                if (criteria.IsMatch((Job)item.Value))
                    jobs.Add((Job)item.Value);
            }
            return jobs;
        }

        public IEnumerator<Job> GetEnumerator()
        {
            foreach (var item in Cache)
                yield return (Job)item.Value;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            // call the generic version
            return this.GetEnumerator();
        }

        public Job Remove(string jobId)
        {
            return Cache.Remove<Job>(jobId);
        }

        public void Update(Job job)
        {
            Cache.Set(job.Id, job);
        }

        public void UpdatExecStatus(Job job)
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
