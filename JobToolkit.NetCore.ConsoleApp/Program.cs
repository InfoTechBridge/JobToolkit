using AnyCache.InMemory;
using AnyCache.Redis;
using AnyCache.Serialization;
using JobToolkit.Core;
using JobToolkit.InMemory;
using JobToolkit.Redis;
using JobToolkit.SQLite;
using Microsoft.Extensions.Caching.Memory;
using ORMToolkit.Core;
using System;

namespace JobToolkit.NetCore.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // In Memory
            //var anyCache = new InMemoryCache();
            //var repository = new CacheJobRepository(anyCache);
            var repository = new InMemoryJobRepository();

            // Redis
            //var serializer = new BinarySerializer();
            //var serializer = new JsonSerializer();
            //var serializer = new MsgPackSerializer();
            //var anyCache = new RedisCache(keyPrefix: "jobToolkit", serializer: serializer);
            //var repository = new CacheJobRepository(anyCache);
            //var repository = new RedisJobRepository();


            ////SQlite
            //var path = Environment.CurrentDirectory;
            //var binIndex = path.IndexOf("\\bin\\");
            //path = path.Substring(0, binIndex);
            //var connectionString = $"Data Source={path}\\App_Data\\MyDatabase.sqlite; Version=3;datetimeformat=CurrentCulture";
            //var dataManager = new DataManagerNoConfilict(connectionString, new ORMToolkit.SQLite.SQLiteDataProviderFactory());
            //OrmToolkitSettings.ObjectFactory = new ORMToolkit.Core.Factories.ObjectFactory2();
            //var repository = new DBJobRepository(dataManager);
            //var repository = new SQLiteJobRepository(connectionString);

            ////SQL Server
            //var repository = new DBJobRepository(dataManager);



            JobManager jobManager = new JobManager(repository);
            JobServer jobServer = new JobServer(repository);

            //jobManager.DequeueAll();

            //jobServer.Start();

            ExpressionTask task = new ExpressionTask(() => Console.WriteLine("Exprission job {0} executed.", 1));
            Job job = new Job(task);
            job.Cron = CronExpression.Minutely();
            string jobId = jobManager.Enqueue(job).Id;
            Job retJob = jobManager.Get(jobId);
            //retJob.DoAction();

            System.Threading.Thread.Sleep(1000);

            Job job1 = jobManager.Schedule(() => Console.WriteLine("Exprission job {0} executed.", 2));
            Job retJob1 = jobManager.Get(job1.Id);
            //retJob1.DoAction();

            Job job2 = jobManager.Schedule(new CustomTask(), AutomaticRetryPolicy.Default);
            Job retJob2 = jobManager.Get(job2.Id);
            //retJob2.DoAction();

            Job job3 = jobManager.Schedule(() => Console.WriteLine($"Exprission job 3 executed at {DateTime.Now}."), "10 * * * * *");
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

            Console.ReadLine();
        }
    }
}
