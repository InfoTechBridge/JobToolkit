# JobToolkit
.NET Job Scheduler


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

Then use following syntax to store jobs on SqlServer:

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

Then use following syntax to store jobs on Oracle database:

```csharp
var repository = new JobToolkit.Repository.Oracle.OracleJobRepository(<oracle connection string>);
JobManager jobManager = new JobManager(repository);
JobServer jobServer = new JobServer(repository);
jobServer.Start();
```
Note: For using oracle database Oracle Client must be installed and counfigured on the machine running JobToolkit.
