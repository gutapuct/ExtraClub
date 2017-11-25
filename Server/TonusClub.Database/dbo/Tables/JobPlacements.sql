CREATE TABLE [dbo].[JobPlacements] (
    [Id]             UNIQUEIDENTIFIER CONSTRAINT [DF_JobPlacements_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [CompanyId]      UNIQUEIDENTIFIER NOT NULL,
    [EmployeeId]     UNIQUEIDENTIFIER NOT NULL,
    [CreatedBy]      UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn]      DATETIME         NOT NULL,
    [JobId]          UNIQUEIDENTIFIER NOT NULL,
    [CategoryId]     UNIQUEIDENTIFIER NOT NULL,
    [ApplyDate]      DATE             NOT NULL,
    [Study]          INT              NOT NULL,
    [TestPeriod]     INT              NOT NULL,
    [Seniority]      INT              NOT NULL,
    [FireDate]       DATE             NULL,
    [FiredById]      UNIQUEIDENTIFIER NULL,
    [IsAsset]        BIT              NOT NULL,
    [FireCause]      NVARCHAR (512)   NULL,
    [DocumentId]     UNIQUEIDENTIFIER NOT NULL,
    [FireDocumentId] UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_JobPlacements] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_JobPlacements_Companies] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Companies] ([CompanyId]),
    CONSTRAINT [FK_JobPlacements_EmployeeCategories] FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[EmployeeCategories] ([Id]),
    CONSTRAINT [FK_JobPlacements_EmployeeDocuments] FOREIGN KEY ([DocumentId]) REFERENCES [dbo].[EmployeeDocuments] ([Id]),
    CONSTRAINT [FK_JobPlacements_EmployeeDocuments_Fire] FOREIGN KEY ([FireDocumentId]) REFERENCES [dbo].[EmployeeDocuments] ([Id]),
    CONSTRAINT [FK_JobPlacements_Employees] FOREIGN KEY ([EmployeeId]) REFERENCES [dbo].[Employees] ([Id]),
    CONSTRAINT [FK_JobPlacements_Jobs] FOREIGN KEY ([JobId]) REFERENCES [dbo].[Jobs] ([Id]),
    CONSTRAINT [FK_JobPlacements_Users_Created] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users] ([UserId]),
    CONSTRAINT [FK_JobPlacements_Users_Fired] FOREIGN KEY ([FiredById]) REFERENCES [dbo].[Users] ([UserId])
);


GO
ALTER TABLE [dbo].[JobPlacements] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

