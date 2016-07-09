using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JobToolkit.Core;

namespace JobToolkit.ConsoleUI
{
    [Serializable]
    public class CustomTask : JobTask
    {
        string Name = "Custom job 1";
        public override void Execute()
        {
            Console.WriteLine(Name);
            throw new ApplicationException();
        }
    }
}
