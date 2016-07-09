﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JobToolkit.Core;
using JobToolkit.DBRepository;
using JobToolkit.Repository.Oracle;
using JobToolkit.Repository.SqlServer;

namespace JobToolkit.ConsoleUI
{
    class Program
    {
        static void Main(string[] args)
        {
            IJobRepository repository = new OracleJobRepository();
            //IJobRepository repository = new SqlJobRepository();
            //IJobRepository repository = new CacheJobRepository();
            JobManager jobManager = new JobManager(repository);
            //JobManager jobManager = JobManager.Default;
            JobServer jobServer = new JobServer(repository);
            //JobServer jobServer = JobServer.Default;
            //IJobServer jobServer = jobManager.JobServer;
            //jobServer.Start();

            ExpressionTask task = new ExpressionTask(() => Console.WriteLine("Exprission job {0}.", 1));
            Job job = new Job(task);
            job.Cron = CronExpression.Minutely();
            string jobId = jobManager.Enqueue(job).Id;
            Job retJob = jobManager.Get(jobId);
            //retJob.DoAction();

            System.Threading.Thread.Sleep(1000);

            Job job1 = jobManager.Schedule(() => Console.WriteLine("Exprission job {0}.", 2), null);
            Job retJob1 = jobManager.Get(job1.Id);
            //retJob1.DoAction();

            Job job2 = jobManager.Schedule(new CustomTask(), AutomaticRetryPolicy.Default);
            Job retJob2 = jobManager.Get(job2.Id);
            //retJob2.DoAction();

            Job job3 = jobManager.Schedule(() => Console.WriteLine("Exprission job {0}.", 3), DateTimeOffset.Now, "* * * * *", null);
            Job retJob3 = jobManager.Get(job3.Id);
            //retJob3.DoAction();

            System.Threading.Thread.Sleep(1000);

            foreach (Job o in jobManager)
            {
                Console.WriteLine("{0} {1} {2} {3}", o.Title, o.Status, o.LastExecutanInfo?.Status, o.LastExecutanInfo?.Duration);
            }

            jobServer.Start();

            System.Threading.Thread.Sleep(90000);

            jobServer.Stop();

            System.Threading.Thread.Sleep(1000);

            foreach (Job o in jobManager)
            {
                Console.WriteLine("{0} {1} {2} {3}", o.Title, o.Status, o.LastExecutanInfo?.Status, o.LastExecutanInfo?.Duration);
            }

            //Console.ReadLine();
        }
    }
}
