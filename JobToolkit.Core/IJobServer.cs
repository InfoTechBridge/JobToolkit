using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobToolkit.Core
{
    public interface IJobServer : IDisposable
    {
        void Start();
        void Stop();
    }
}
