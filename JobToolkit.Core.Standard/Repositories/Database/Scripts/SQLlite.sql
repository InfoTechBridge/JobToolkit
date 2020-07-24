CREATE TABLE Job (
    Id                 VARCHAR2 (50)              NOT NULL  PRIMARY KEY,
    Title              NVARCHAR2 (100),
    TaskData           NCLOB                      NOT NULL,
    Priority           INT                        NOT NULL,
    Enabled            BOOLEAN                    NOT NULL,
    Status             INT                        NOT NULL,
    ScheduleTime       [TIMESTAMP WITH TIME ZONE] NOT NULL,
    Cron               VARCHAR2 (50),
    NextScheduleTime   [TIMESTAMP WITH TIME ZONE],
    ScheduleEndTime    [TIMESTAMP WITH TIME ZONE],
    LastExecutanInfoId VARCHAR2 (50),
    MaxRetryAttempts   INT,
    Owner              NVARCHAR2 (50),
    Description        NVARCHAR2 (500),
    CreateTime         [TIMESTAMP WITH TIME ZONE] NOT NULL,
    UpdateTime         [TIMESTAMP WITH TIME ZONE]
);

CREATE TABLE JobExec (
    Id            VARCHAR2 (50)              NOT NULL
                                             PRIMARY KEY,
    JobId         VARCHAR2 (50)              NOT NULL,
    RetryNumber   INTEGER                    NOT NULL,
    StartTime     [TIMESTAMP WITH TIME ZONE] NOT NULL,
    EndTime       [TIMESTAMP WITH TIME ZONE],
    Status        INTEGER                    NOT NULL,
    Description   NVARCHAR2 (500),
    NextRetryTime [TIMESTAMP WITH TIME ZONE]
);
