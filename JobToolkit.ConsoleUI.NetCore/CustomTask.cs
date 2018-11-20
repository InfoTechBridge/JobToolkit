using JobToolkit.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobToolkit.ConsoleUI.NetCore
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
