using JobToolkit.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobToolkit.NetCore.ConsoleApp
{
    [Serializable]
    public class CustomTask : JobTask
    {
        private string Name = "Custom job 1 executed";
        public override void Execute()
        {
            Console.WriteLine(Name);
            throw new ApplicationException();
        }
    }
}
