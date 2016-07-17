using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobToolkit.Core
{
    public interface IJobRepository : IEnumerable<Job>
    {        
        string Add(Job job);
        Job Get(string jobId);
        void Update(Job job);
        void UpdatExecStatus(Job job);
        Job Remove(string jobId);

        List<Job> GetAll();
        List<Job> GetAll(JobDataQueryCriteria criteria);
    }
}
