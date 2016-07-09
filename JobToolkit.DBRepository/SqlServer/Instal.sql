USE [JobToolkit]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

/****** Object:  Table [dbo].[Job]    Script Date: 2/16/2016 12:21:21 PM ******/
CREATE TABLE [dbo].[Job](
	[Id] [varchar](50) NOT NULL,
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
	[UpdateTime] [datetimeoffset](7) NULL,
 CONSTRAINT [PK_Job] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/****** Object:  Table [dbo].[ExecutanInfo]    Script Date: 2/16/2016 12:24:21 PM ******/
CREATE TABLE [dbo].[ExecutanInfo](
	[Id] [varchar](50) NOT NULL,
	[JobId] [varchar](50) NOT NULL,
	[RetryNumber] [int] NOT NULL,
	[StartTime] [datetimeoffset](7) NOT NULL,
	[EndTime] [datetimeoffset](7) NULL,
	[Status] [int] NOT NULL,
	[Description] [nvarchar](500) NULL,
	[NextRetryTime] [datetimeoffset](7) NULL,
 CONSTRAINT [PK_ExecutanInfo] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_PADDING OFF
GO

