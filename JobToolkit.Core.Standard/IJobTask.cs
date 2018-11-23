using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobToolkit.Core
{
    public interface IJobTask
    {
        string Title { get; }
        string Description { get; set; }
        void Execute();
    }
}
