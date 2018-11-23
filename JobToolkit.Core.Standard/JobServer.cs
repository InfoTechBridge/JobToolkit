using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
//using JobToolkit.Core.Configuration;
//using JobToolkit.Core.Factory;

namespace JobToolkit.Core
{
    public class JobServer : IJobServer, IDisposable
    {
        public IJobRepository Repository { get; }
        CancellationTokenSource cts;

        private static JobServer defaultServer;
        
        //private JobServer()
        //    : this(JobToolkitConfiguration.Config.JobServerConfigurations[JobToolkitConfiguration.Config.DefaultServer])
        //{

        //}

        public JobServer(IJobRepository repository)
        {
            Repository = repository;
        }

        //public JobServer(JobServerConfiguration configuration)
        //{
        //    var repositoryName = string.IsNullOrEmpty(configuration.Repository.Trim()) ? JobToolkitConfiguration.Config.DefaultRepository : configuration.Repository;
        //    Repository = JobToolkitFactory.CreateJobRepository(JobToolkitConfiguration.Config.RepositoryConfigurations[repositoryName]);
        //}

        //public static JobServer Default
        //{
        //    get
        //    {
        //        if (defaultServer == null)
        //            defaultServer = new JobServer();

        //        return defaultServer;
        //    }
        //}

        public void Start()
        {
            // Create the token source.
            cts = new CancellationTokenSource();
            // Pass the token to the cancelable operation.
            StartMainTaskAsync(cts.Token);
        }

        public void Stop()
        {
            cts.Cancel();
            cts.Dispose();
        }

        private Task StartMainTaskAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                try
                {
                    while (true)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        //if (cancellationToken.IsCancellationRequested)
                        //{
                        //    //break;
                        //    //throw new OperationCanceledException(cancellationToken); // acknowledge cancellation
                        //}

                       
                        JobDataQueryCriteria criteria = new JobDataQueryCriteria()
                        {
                            Status = JobStatus.Scheduled,
                            NextScheduleTimeIsNull = false,
                            NextScheduleTimeLe = DateTimeOffset.Now,
                            SortExpression = "NextScheduleTime, Priority",
                            Take = 10,
                        };

                        IList<Job> jobs = Repository.GetAll(criteria);
                        if (jobs.Count <= 0)
                        {
                            System.Threading.Thread.Sleep(200);
                            continue;
                        }

                        Parallel.ForEach(jobs, (j) =>
                        {
                            try
                            {
                                ExecuteJobTask(j);
                                //j.DoAction();
                            }
                            catch (Exception ex)
                            {
                                //log
                                Console.WriteLine(ex.Message);
                            }
                        });
                    }
                }
                catch (OperationCanceledException)
                {
                    //cleanup after cancellation if required...
                    Console.WriteLine("Operation was canceled as expected.");
                }
                catch (AggregateException ae)
                {
                    foreach (Exception e in ae.InnerExceptions)
                    {
                        if (e is TaskCanceledException)
                            Console.WriteLine("Unable to compute mean: {0}",
                                              ((TaskCanceledException)e).Message);
                        else
                            Console.WriteLine("Exception: " + e.GetType().Name);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }, cancellationToken);
        }

        private void ExecuteJobTask(Job job)
        {
            JobExecutanInfo execInfo = new JobExecutanInfo()
            {
                JobId = job.Id,
                RetryNumber =  1,
                Status = JobExecutanStatus.Running,
                StartTime = DateTimeOffset.Now,
            };

            if (job.LastExecutanInfo != null && job.LastExecutanInfo.NextRetryTime != null)
                execInfo.RetryNumber = job.LastExecutanInfo.RetryNumber + 1;
            job.LastExecutanInfo = execInfo;
            job.Status = JobStatus.Runnung;
            job.UpdateTime = DateTimeOffset.Now;

            // save status befor executation
            Repository.UpdatExecStatus(job);

            try
            {
                job.Task?.Execute();

                job.LastExecutanInfo.Status = JobExecutanStatus.Success;
                job.LastExecutanInfo.Description = JobExecutanStatus.Success.ToString();
            }
            catch (Exception ex)
            {
                job.LastExecutanInfo.Status = JobExecutanStatus.Failed;
                job.LastExecutanInfo.Description = ex.Message;
            }
            finally
            {
                job.LastExecutanInfo.EndTime = DateTimeOffset.Now;

                #region Reschedule
                job.NextScheduleTime = job.GetNextScheduleTime(job.NextScheduleTime.GetValueOrDefault(DateTimeOffset.Now));
                if (job.LastExecutanInfo.Status == JobExecutanStatus.Failed
                        && job.AutomaticRetry != null && job.AutomaticRetry.MaxRetryAttempts > 0
                        && job.LastExecutanInfo.RetryNumber < job.AutomaticRetry.MaxRetryAttempts)
                {
                    job.LastExecutanInfo.NextRetryTime = DateTimeOffset.Now.AddSeconds(AutomaticRetryPolicy.SecondsToDelay(job.LastExecutanInfo.RetryNumber));

                    if (job.NextScheduleTime.HasValue && job.NextScheduleTime <= job.LastExecutanInfo.NextRetryTime)
                        job.LastExecutanInfo.NextRetryTime = null;
                    else
                        job.NextScheduleTime = job.LastExecutanInfo.NextRetryTime;
                }
                #endregion Reschedule

                job.Status = job.NextScheduleTime.HasValue ? JobStatus.Scheduled : JobStatus.Finished;
                job.UpdateTime = DateTimeOffset.Now;

                // save status after executation
                Repository.UpdatExecStatus(job);
            }
        }
        
        public void Dispose()
        {
            cts?.Cancel();
            cts?.Dispose();
        }

        private static TResult RunSync<TResult>(Func<Task<TResult>> func)
        {
            return Task.Run<Task<TResult>>(func).Unwrap().GetAwaiter().GetResult();
        }
        
    }
}
