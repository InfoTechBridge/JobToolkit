
CREATE TABLE Job(
    Id varchar2(50) NOT NULL PRIMARY KEY,
    Title nvarchar2(100) NULL,
    TaskData nclob NOT NULL, 
    Priority number(2) NOT NULL,
    Enabled number(1) NOT NULL,
    Status number(2) NOT NULL,
    ScheduleTime TIMESTAMP WITH TIME ZONE NOT NULL,
    Cron varchar2(50) NULL,
    NextScheduleTime TIMESTAMP WITH TIME ZONE NULL,
    ScheduleEndTime TIMESTAMP WITH TIME ZONE NULL,
    LastExecutanInfoId varchar2(50) NULL,
    MaxRetryAttempts number(9) NULL,
    Owner nvarchar2(50) NULL,
    Description nvarchar2(500) NULL,
    CreateTime TIMESTAMP WITH TIME ZONE NOT NULL,
    UpdateTime TIMESTAMP WITH TIME ZONE NULL
 
);

CREATE TABLE JobExec(
    Id varchar2(50) NOT NULL PRIMARY KEY,
    JobId varchar2(50) NOT NULL,
    RetryNumber number(9) NOT NULL,
    StartTime TIMESTAMP WITH TIME ZONE NOT NULL,
    EndTime TIMESTAMP WITH TIME ZONE NULL,
    Status number(2) NOT NULL,
    Description nvarchar2(500) NULL,
    NextRetryTime TIMESTAMP WITH TIME ZONE NULL
);