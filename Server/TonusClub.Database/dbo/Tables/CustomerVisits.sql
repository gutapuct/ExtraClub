CREATE TABLE [dbo].[CustomerVisits] (
    [Id]         UNIQUEIDENTIFIER CONSTRAINT [DF_ClientVisits_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [InTime]     DATETIME         NOT NULL,
    [OutTime]    DATETIME         NULL,
    [CreatedBy]  UNIQUEIDENTIFIER NOT NULL,
    [CompanyId]  UNIQUEIDENTIFIER NOT NULL,
    [CustomerId] UNIQUEIDENTIFIER NOT NULL,
    [DivisionId] UNIQUEIDENTIFIER NOT NULL,
    [Receipt]    NVARCHAR (MAX)   NULL,
    CONSTRAINT [PK_ClientVisits] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ClientVisits_Users] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users] ([UserId]),
    CONSTRAINT [FK_CustomerVisits_Customers] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customers] ([Id]),
    CONSTRAINT [FK_CustomerVisits_Divisions] FOREIGN KEY ([DivisionId]) REFERENCES [dbo].[Divisions] ([Id])
);


GO
ALTER TABLE [dbo].[CustomerVisits] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);


GO
CREATE NONCLUSTERED INDEX [IX_CustomerVisits]
    ON [dbo].[CustomerVisits]([InTime] ASC);


GO
CREATE NONCLUSTERED INDEX [Workflow_ProcessEvents_Index]
    ON [dbo].[CustomerVisits]([OutTime] ASC, [CustomerId] ASC);


GO
CREATE NONCLUSTERED INDEX [CustomerVisits_StartupIndex]
    ON [dbo].[CustomerVisits]([DivisionId] ASC)
    INCLUDE([InTime]);


GO
CREATE NONCLUSTERED INDEX [IX_CRM_REPORT_3]
    ON [dbo].[CustomerVisits]([InTime] ASC)
    INCLUDE([DivisionId]);


GO
CREATE NONCLUSTERED INDEX [IX_CompanyId]
    ON [dbo].[CustomerVisits]([CompanyId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CustomerID]
    ON [dbo].[CustomerVisits]([CustomerId] ASC);

