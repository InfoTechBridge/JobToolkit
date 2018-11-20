using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobToolkit.Core
{
    public enum JobStatus
    {
        Scheduled = 0,
        Queued = 1,
        Runnung = 2,
        Finished = 3,
        Removed = 4,
    }
}
