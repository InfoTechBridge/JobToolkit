# JobToolkit
[![License](http://img.shields.io/:license-MIT-blue.svg)](https://raw.githubusercontent.com/giacomelli/JobSharp/master/LICENSE)


.NET Crontab Background Job Scheduler for .Netframework, .Net Core and Xamarin

Features
===
- Fire-and-forget jobs 
- One time or recurring jobs support by intervals
- Job scheduling using Crontab expressions
- Immidiate start jobs and delayed start jobs
- Automatic retries of failed jobs (configurable)
- Lambada syntax jobs and custom jobs by inheritance
- Failure in one job will not affect the others
- Multiple repository support
    - In-memory fast repository for job storage
    - SqlServer database repository for job storage
    - Oracle database repository for job storage
    - SQLite database repository for job storage
    - Redis cache repository for job storage
    - Custom storage support by writing custom repositories such as (MySql, PostGre Sql and ... )
- Easy instalation and configuration
- Supports any type of projects (Web, Win, Console, Win Service and ...)
- Supports DotNet Framework, DotNet Core and Xamarin projects
- Allow Sepration between job producer and job executer on diffrent application or servers (Multiple producers and one executer)

Usage
------

```csharp
var repository = new InMemoryJobRepository();
JobManager jobManager = new JobManager(repository);
JobServer jobServer = new JobServer(repository);
jobServer.Start();
```

**One time executation tasks**

```csharp
jobManager.Schedule(() => Console.WriteLine("Exprission job {0}.", 1));
```

**Automatic retray**

Automaticly retray when fail happens in job execution time. In following command job executes for 10 times if fail happens.

```csharp
jobManager.Schedule(() => Console.WriteLine("Exprission job {0}.", 2), AutomaticRetryPolicy.Default);
```

**Cron tab executation tasks**

```csharp
jobManager.Schedule(() => Console.WriteLine("Exprission job {0}.", 3), DateTimeOffset.Now, "* * * * *", null);
```
Determining end time for cron tab jobs. The following command executes job for 90 days.

```csharp
jobManager.Schedule(() => Console.WriteLine("Exprission job {0}.", 3), DateTimeOffset.Now, "* * * * *", DateTimeOffset.Now.AddDays(90));
```

**Custom jobs**

Define your custom job by inheriting your class from JobToolkit.Core.JobTask.

```csharp
[Serializable]
public class CustomTask : JobTask
{
    string Name = "Custom job 1";
    public override void Execute()
    {
        Console.WriteLine(Name);
    }
}
```

Then use folowing syntax to schedule one instance of your job.

```csharp
CustomTask task = new CustomTask();
Job job = new Job(task);
job.Cron = CronExpression.Minutely();
string jobId = jobManager.Enqueue(job).Id;
```

Or use following syntax.

```csharp
Job job = jobManager.Schedule(new CustomTask(), AutomaticRetryPolicy.Default);
```

Installation
-------------

JobToolkit stores jobs in diffrent type of storage such as InMemory, Redis, Sql Server, Oracle and SQLite databases and is available as a NuGet package. 

**In Memory job repository**

JobToolkit saves jobs in memmory cache and everything will be cleared when the application ends.

Use following NuGet command to install JobToolkit InMemory library. 

```
PM> Install-Package JobToolkit.InMemory
```

Then use following syntax to store jobs in memory:

```csharp
var repository = new InMemoryJobRepository();
JobManager jobManager = new JobManager(repository);
JobServer jobServer = new JobServer(repository);
jobServer.Start();
```

**Sql server**

Use following NuGet command to install JobToolkit SqlServer library.  

```
PM> Install-Package JobToolkit.SqlServer
```

then for creating requared tables on your SqlServer database please run following script in your database:

```
JobToolkit/JobToolkit.SqlServer/Scripts/SqlServer.sql
```

Then use following syntax to store jobs on SqlServer:

```csharp
var repository = new JobToolkit.SqlServer.SqlJobRepository(<sql server connection string>);
JobManager jobManager = new JobManager(repository);
JobServer jobServer = new JobServer(repository);
jobServer.Start();
```

**Oracle**

Use following NuGet command to install JobToolkit Oracle library.  

```
PM> Install-Package JobToolkit.Oracle
```

then for creating requared tables on your Oracle database please run following script in your database:

```
JobToolkit/JobToolkit.Oracle/Scripts/Oracle.sql
```

Then use following syntax to store jobs on Oracle:

```csharp
var repository = new JobToolkit.Oracle.OracleJobRepository(<sql server connection string>);
JobManager jobManager = new JobManager(repository);
JobServer jobServer = new JobServer(repository);
jobServer.Start();
```

Note: jobToolkit for Oracle database uses OracleManagement library and not reauares to install Oracle Client on the machine running JobToolkit.


**SQLite**

Use following NuGet command to install JobToolkit SQLite library.  

```
PM> Install-Package JobToolkit.SQLite
```

then for creating requared tables on your SQLite database please run following script in your database:

```
JobToolkit/JobToolkit.SQLite/Scripts/SQLite.sql
```

Then use following syntax to store jobs on SQLite:

```csharp
var repository = new JobToolkit.SQLite.SQLiteJobRepository(<sql server connection string>);
JobManager jobManager = new JobManager(repository);
JobServer jobServer = new JobServer(repository);
jobServer.Start();
```

**Redis Cache**

Use following NuGet command to install JobToolkit Redis library.  

```
PM> Install-Package JobToolkit.Redis
```

Then use following syntax to store jobs on Redis:

```csharp
var repository = new JobToolkit.Redis.RedisJobRepository(<sql server connection string>);
JobManager jobManager = new JobManager(repository);
JobServer jobServer = new JobServer(repository);
jobServer.Start();
```

Dependency Injection
--------------------

JobToolkit supports Dependency Injection. Following example shows how to use JobToolkit by .Net Core injection mechanism.

**In Memory**

```csharp
service.AddSingleton<IJobRepository>(s =>
{
    return new InMemoryJobRepository();
});

Note: Remember to register InMemoryJobRepository as singleton to make sure using same repository instance in whole of your project.

service.AddTransient<IScheduler>(s =>
{
    var repository = s.GetService<IJobRepository>();
    return new JobManager(repository);
});
```

service.AddSingleton<IJobServer>(s =>
{
    var repository = s.GetService<IJobRepository>();
    return new JobManager(JobServer);
});
```

Note: Remember to register JobServer as singleton to make sure using same JobServer instance in whole of your project.

Now you can inject JobToolkit at runtime into your services/controllers:

```csharp
public class EmployeesController 
{
    private readonly IScheduler _scheduler;

    public EmployeesController(IScheduler _scheduler)
    {
        _scheduler = _scheduler;
    }

    // use _scheduler
}
```

Configuration for .Net Framework projects
-----------------------------------------

If you dont like to hardcode yor repository type in your code you can use Web.config or App.config file to manage repositories.

For configuring your JobToolkit, at first add folowing line inside 'configSections' of your project config file.

```xml
<section name="jobToolkit" type="JobToolkit.Core.Configuration.JobToolkitConfiguration, JobToolkit.Core" />
```

The 'configSections' is first element in your config file and if there is no, add following lines to top of your config file inside the 'configuration' element.

```xml
<configSections>
    <section name="jobToolkit" type="JobToolkit.Core.Configuration.JobToolkitConfiguration, JobToolkit.Core" />
</configSections>
```

In the 'connectionStrings' section of your config fille add your database connection strings, such as following strings.

```xml
<connectionStrings>
    <clear />
    <add name="sqlConnection" connectionString="Data Source=.\SQLEXPRESS;Initial Catalog=JobToolkit;Integrated Security=True" providerName="System.Data.ProviderName" />
    <add name="oracleConnection" connectionString="User ID=jobtoolkit;Password=jobtoolkit;Data Source=XE;Persist Security Info=True;enlist=true" providerName="System.Data.ProviderName" />
  </connectionStrings>
```

Then add following tags to your config file inside 'configuration'.

```xml
<jobToolkit defaultManager="defaultManager" defaultServer="defaultServer" defaultRepository="cacheRepository" >
    <managers>
      <add name="defaultManager" type="JobToolkit.Core.JobManager" repository="" />
    </managers>
    <servers>
      <add name="defaultServer" type="JobToolkit.Core.JobServer" repository="" />
    </servers>
    <repositories>
      <add name="cacheRepository" type="JobToolkit.Core.CacheJobRepository" connectionString="" />
      <add name="sqlRepository" type="JobToolkit.Repository.SqlServer.SqlJobRepository, JobToolkit.Repository.SqlServer" connectionString="sqlConnection" />
      <add name="oracleRepository" type="JobToolkit.Repository.Oracle.OracleJobRepository, JobToolkit.Repository.Oracle" connectionString="oracleConnection" />
    </repositories>
</jobToolkit>
```
  
In the 'jobToolkit' config section 'defaultRepository' attribute determainse witch repository is your default repository. By default it is 'cacheRepository' that says JobToolkit uses in memory repository. By changing value of defaultRepository to 'sqlRepository' or 'oracleRepository', you will connect to diffrent repositorie with out any change in your programs code.

Then in your program just use following syntax to connect to your favorite repository.
  
```csharp
JobManager jobManager = JobManager.Default;
JobServer jobServer = JobServer.Default;
jobServer.Start();
```

For commplete axample please see 'JobToolkit/JobToolkit.ConsoleUI' project in this site.
  
