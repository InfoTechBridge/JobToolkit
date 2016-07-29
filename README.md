# JobToolkit
[![License](http://img.shields.io/:license-MIT-blue.svg)](https://raw.githubusercontent.com/giacomelli/JobSharp/master/LICENSE)


.NET Job Scheduler

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
    - Custom storage support by writing custom repositories such as (MySql, PostGre Sql, Redis and ... )
- Easy instalation and configuration
- Any tipe of projects (Web, Win, Console, Win Service and ...)
- Allow Sepration between job producer and job executer on diffrent application or servers (Multiple producers and one executer)

Usage
------

```csharp
JobManager jobManager = JobManager.Default;
JobServer jobServer = JobServer.Default;
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

Then using folowing syntax to schedule them.

```csharp
CustomTask task = new CustomTask();
Job job = new Job(task);
job.Cron = CronExpression.Minutely();
string jobId = jobManager.Enqueue(job).Id;
```

Or using following syntax.

```csharp
Job job = jobManager.Schedule(new CustomTask(), AutomaticRetryPolicy.Default);
```

Installation
-------------

JobToolkit is available as a NuGet package. For installing by the NuGet Package Console window, enter following command:

```
PM> Install-Package JobToolkit.Core
```

By default JobToolkit saves jobs in memmory cache and by ending the application everything will be cleared. For storing job permanently, JobToolkit at the moment supports SqlServer and Oracle databases.

**Sql server**

For creating requared tables on your SqlServer database please run following script in your database:

```
JobToolkit/JobToolkit.Repository.SqlServer/Scripts/Instal.sql
```

And then use following NuGet command to install SqlServer repository library. 

```
PM> Install-Package JobToolkit.Repository.SqlServer
```

Then use following syntax to store jobs on SqlServer or see Configuration section of this document:

```csharp
var repository = new JobToolkit.Repository.SqlServer.SqlJobRepository(<sql server connection string>);
JobManager jobManager = new JobManager(repository);
JobServer jobServer = new JobServer(repository);
jobServer.Start();
```

**Oracle**

For creating requared tables on your Oracle database please run following script in your database:

```
JobToolkit/JobToolkit.Repository.Oracle/Scripts/Instal.sql
```

And then use following NuGet command to install Oracle repository library. 

```
PM> Install-Package JobToolkit.Repository.Oracle
```

Then use following syntax to store jobs on Oracle database or see Configuration section of this document:

```csharp
var repository = new JobToolkit.Repository.Oracle.OracleJobRepository(<oracle connection string>);
JobManager jobManager = new JobManager(repository);
JobServer jobServer = new JobServer(repository);
jobServer.Start();
```
Note: For using oracle database Oracle Client must be installed and counfigured on the machine running JobToolkit.

Configuration
--------------

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
  
