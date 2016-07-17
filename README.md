# JobToolkit
.NET Job Scheduler


Usage
------

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

Cron tab support.

```csharp
jobManager.Schedule(() => Console.WriteLine("Exprission job {0}.", 3), DateTimeOffset.Now, "* * * * *", null);
```
Determining end time for cron tab jobs. The following command executes job for 90 days.

```csharp
jobManager.Schedule(() => Console.WriteLine("Exprission job {0}.", 3), DateTimeOffset.Now, "* * * * *", DateTimeOffset.Now.AddDays(90));
```
