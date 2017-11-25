CREATE TABLE [dbo].[SolariumVisits] (
    [Id]         UNIQUEIDENTIFIER CONSTRAINT [DF_SolariumVisits_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [CompanyId]  UNIQUEIDENTIFIER NOT NULL,
    [DivisionId] UNIQUEIDENTIFIER NOT NULL,
    [CustomerId] UNIQUEIDENTIFIER NOT NULL,
    [SolariumId] UNIQUEIDENTIFIER NOT NULL,
    [Status]     SMALLINT         CONSTRAINT [DF_SolariumVisits_Status] DEFAULT ((0)) NOT NULL,
    [Comment]    NVARCHAR (256)   NULL,
    [CreatedBy]  UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn]  DATETIME         NOT NULL,
    [VisitDate]  DATETIME         NOT NULL,
    [Amount]     INT              CONSTRAINT [DF_SolariumVisits_Amount] DEFAULT ((0)) NOT NULL,
    [TicketId]   UNIQUEIDENTIFIER NULL,
    [Cost]       MONEY            NULL,
    CONSTRAINT [PK_SolariumVisits] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_SolariumVisits_Companies] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Companies] ([CompanyId]),
    CONSTRAINT [FK_SolariumVisits_Customers] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customers] ([Id]),
    CONSTRAINT [FK_SolariumVisits_DictionaryInfo] FOREIGN KEY ([DivisionId]) REFERENCES [dbo].[Divisions] ([Id]),
    CONSTRAINT [FK_SolariumVisits_Solariums] FOREIGN KEY ([SolariumId]) REFERENCES [dbo].[Solariums] ([Id]),
    CONSTRAINT [FK_SolariumVisits_Tickets] FOREIGN KEY ([TicketId]) REFERENCES [dbo].[Tickets] ([Id]),
    CONSTRAINT [FK_SolariumVisits_Users] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users] ([UserId])
);


GO
ALTER TABLE [dbo].[SolariumVisits] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);


GO
CREATE NONCLUSTERED INDEX [IX_SolariumVisitsByCustomer]
    ON [dbo].[SolariumVisits]([TicketId] ASC)
    INCLUDE([Id], [CompanyId], [DivisionId], [CustomerId], [SolariumId], [Status], [Comment], [CreatedBy], [CreatedOn], [VisitDate], [Amount], [Cost]);


GO
CREATE NONCLUSTERED INDEX [IX_CompanyId]
    ON [dbo].[SolariumVisits]([CompanyId] ASC);

