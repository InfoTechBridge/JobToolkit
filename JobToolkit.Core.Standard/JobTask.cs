using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobToolkit.Core
{
    [Serializable]
    public abstract class JobTask : IJobTask
    {
        public virtual string Title
        {
            get { return this.GetType().ToString(); }
        }
        public virtual string Description { get; set; }
        public abstract void Execute();
    }
}
