CREATE TABLE [dbo].[EmployeeTrips] (
    [Id]          UNIQUEIDENTIFIER CONSTRAINT [DF_EmployeeTrips_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [CompanyId]   UNIQUEIDENTIFIER NOT NULL,
    [CreatedBy]   UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn]   DATETIME         NOT NULL,
    [Number]      INT              NOT NULL,
    [BeginDate]   DATE             NOT NULL,
    [EndDate]     DATE             NOT NULL,
    [EmployeeId]  UNIQUEIDENTIFIER NOT NULL,
    [Destination] NVARCHAR (512)   NOT NULL,
    [Base]        NVARCHAR (512)   NOT NULL,
    [Target]      NVARCHAR (512)   NOT NULL,
    [DocumentId]  UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_EmployeeTrips] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_EmployeeTrips_Companies] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Companies] ([CompanyId]),
    CONSTRAINT [FK_EmployeeTrips_EmployeeDocuments] FOREIGN KEY ([DocumentId]) REFERENCES [dbo].[EmployeeDocuments] ([Id]),
    CONSTRAINT [FK_EmployeeTrips_Employees] FOREIGN KEY ([EmployeeId]) REFERENCES [dbo].[Employees] ([Id]),
    CONSTRAINT [FK_EmployeeTrips_Users] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users] ([UserId])
);


GO
ALTER TABLE [dbo].[EmployeeTrips] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

