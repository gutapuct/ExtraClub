CREATE TABLE [dbo].[VacationPreferences] (
    [Id]         UNIQUEIDENTIFIER CONSTRAINT [DF_VacationPreferences_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [CompanyId]  UNIQUEIDENTIFIER NOT NULL,
    [EmployeeId] UNIQUEIDENTIFIER NOT NULL,
    [StartDate]  DATE             NOT NULL,
    [EndDate]    DATE             NOT NULL,
    [CreatedOn]  DATETIME         NOT NULL,
    [PrefType]   SMALLINT         NOT NULL,
    CONSTRAINT [PK_VacationPreferences] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_VacationPreferences_Employees] FOREIGN KEY ([EmployeeId]) REFERENCES [dbo].[Employees] ([Id])
);


GO
ALTER TABLE [dbo].[VacationPreferences] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

