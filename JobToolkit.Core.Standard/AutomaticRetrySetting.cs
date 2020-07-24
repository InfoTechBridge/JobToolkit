using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobToolkit.Core
{
    [Serializable]
    public class AutomaticRetryPolicy
    {
        public static readonly int DefaultRetryAttempts = 10;
        //public static readonly TimeSpan DefaultRetryTimeFrame = new TimeSpan(1, 0, 0);

        public int MaxRetryAttempts { get; set; } = DefaultRetryAttempts;
        //public TimeSpan MaxRetryTimeFrame { get; set; } = DefaultRetryTimeFrame;

        public static AutomaticRetryPolicy Default
        {
            get
            {
                return new AutomaticRetryPolicy();
            }
        }

        public static int SecondsToDelay(long retryNumber)
        {
            var random = new Random();
            return (int)Math.Round(
                Math.Pow(retryNumber - 1, 4) + 15 + (random.Next(30) * (retryNumber)));
        }
    }
}
