CREATE TABLE [dbo].[EmployeeVacations] (
    [Id]           UNIQUEIDENTIFIER CONSTRAINT [DF_EmployeeVacations_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [CompanyId]    UNIQUEIDENTIFIER NOT NULL,
    [EmployeeId]   UNIQUEIDENTIFIER NOT NULL,
    [CreatedBy]    UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn]    DATETIME         NOT NULL,
    [BeginDate]    DATE             NOT NULL,
    [EndDate]      DATE             NOT NULL,
    [VacationType] SMALLINT         NOT NULL,
    [DocumentId]   UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_EmployeeVacations] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_EmployeeVacations_Companies] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Companies] ([CompanyId]),
    CONSTRAINT [FK_EmployeeVacations_EmployeeDocuments] FOREIGN KEY ([DocumentId]) REFERENCES [dbo].[EmployeeDocuments] ([Id]),
    CONSTRAINT [FK_EmployeeVacations_Employees] FOREIGN KEY ([EmployeeId]) REFERENCES [dbo].[Employees] ([Id]),
    CONSTRAINT [FK_EmployeeVacations_Users] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users] ([UserId])
);


GO
ALTER TABLE [dbo].[EmployeeVacations] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

