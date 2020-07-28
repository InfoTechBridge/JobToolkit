
CREATE TABLE [dbo].[Job](
	[Id] [varchar](50) NOT NULL PRIMARY KEY,
	[Title] [nvarchar](100) NULL,
	[TaskData] [ntext] NOT NULL, -- varbinary(MAX) not supported by Sql Express
	[Priority] [int] NOT NULL,
	[Enabled] [bit] NOT NULL,
	[Status] [int] NOT NULL,
	[ScheduleTime] [datetimeoffset](7) NOT NULL,
	[Cron] [varchar](50) NULL,
	[NextScheduleTime] [datetimeoffset](7) NULL,
	[ScheduleEndTime] [datetimeoffset](7) NULL,
	[LastExecutanInfoId] [varchar](50) NULL,
	[MaxRetryAttempts] [int] NULL,
	[Owner] [nvarchar](50) NULL,
	[Description] [nvarchar](500) NULL,
	[CreateTime] [datetimeoffset](7) NOT NULL,
	[UpdateTime] [datetimeoffset](7) NULL
) 
GO


CREATE TABLE [dbo].[JobExec](
	[Id] [varchar](50) NOT NULL PRIMARY KEY,
	[JobId] [varchar](50) NOT NULL,
	[RetryNumber] [int] NOT NULL,
	[StartTime] [datetimeoffset](7) NOT NULL,
	[EndTime] [datetimeoffset](7) NULL,
	[Status] [int] NOT NULL,
	[Description] [nvarchar](500) NULL,
	[NextRetryTime] [datetimeoffset](7) NULL
) 
GO

