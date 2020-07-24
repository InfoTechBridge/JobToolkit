using AnyCache.Redis;
using AnyCache.Serialization;
using JobToolkit.AnyCacheRepository;
using JobToolkit.Core;
using System;

namespace JobToolkit.ConsoleUI.NetCore
{
    class Program
    {
        static void Main(string[] args)
        {
            //IJobRepository repository = new OracleJobRepository();
            //IJobRepository repository = new SqlJobRepository();
            //IJobRepository repository = new CacheJobRepository();
            //IJobRepository repository = new AnyCacheJobRepository();

            //var repository = new AnyCacheJobRepository(new RedisCache(keyPrefix: "job", serializer: new BinarySerializer()));
            var repository = new AnyCacheJobRepository(new RedisCache(keyPrefix: "job", serializer: new JsonSerializer()));
            //var repository = new AnyCacheJobRepository(new RedisCache(keyPrefix: "job", serializer: new MsgPackSerializer()));

            var jobManager = new JobManager(repository);
            var jobServer = new JobServer(repository);

            //var jobManager = JobManager.Default;
            //var jobServer = JobServer.Default;

            jobManager.DequeueAll();

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
