CREATE TABLE [dbo].[TreatmentEvents] (
    [Id]                UNIQUEIDENTIFIER CONSTRAINT [DF_TreatmentEvents_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [CompanyId]         UNIQUEIDENTIFIER NOT NULL,
    [DivisionId]        UNIQUEIDENTIFIER NOT NULL,
    [CustomerId]        UNIQUEIDENTIFIER NOT NULL,
    [VisitDate]         DATETIME         NOT NULL,
    [VisitStatus]       SMALLINT         CONSTRAINT [DF_TreatmentEvents_VisitStatus] DEFAULT ((0)) NOT NULL,
    [CreatedBy]         UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn]         DATETIME         CONSTRAINT [DF_TreatmentEvents_CreatedOn] DEFAULT (getdate()) NOT NULL,
    [TicketId]          UNIQUEIDENTIFIER NULL,
    [ModifiedBy]        UNIQUEIDENTIFIER NULL,
    [TreatmentConfigId] UNIQUEIDENTIFIER NOT NULL,
    [TreatmentId]       UNIQUEIDENTIFIER NOT NULL,
    [ProgramId]         UNIQUEIDENTIFIER NULL,
    [CustomColorId]     INT              NULL,
    [Comment]           NVARCHAR (MAX)   NULL,
    CONSTRAINT [PK_TreatmentEvents] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_TreatmentEvents_Customers] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customers] ([Id]),
    CONSTRAINT [FK_TreatmentEvents_Divisions] FOREIGN KEY ([DivisionId]) REFERENCES [dbo].[Divisions] ([Id]),
    CONSTRAINT [FK_TreatmentEvents_Tickets] FOREIGN KEY ([TicketId]) REFERENCES [dbo].[Tickets] ([Id]),
    CONSTRAINT [FK_TreatmentEvents_TreatmentConfig] FOREIGN KEY ([TreatmentConfigId]) REFERENCES [dbo].[TreatmentConfig] ([Id]),
    CONSTRAINT [FK_TreatmentEvents_TreatmentPrograms] FOREIGN KEY ([ProgramId]) REFERENCES [dbo].[TreatmentPrograms] ([Id]),
    CONSTRAINT [FK_TreatmentEvents_Treatments] FOREIGN KEY ([TreatmentId]) REFERENCES [dbo].[Treatments] ([Id]),
    CONSTRAINT [FK_TreatmentEvents_Users] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users] ([UserId]),
    CONSTRAINT [FK_TreatmentEvents_Users1] FOREIGN KEY ([ModifiedBy]) REFERENCES [dbo].[Users] ([UserId])
);


GO
ALTER TABLE [dbo].[TreatmentEvents] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);


GO
CREATE NONCLUSTERED INDEX [IX_TicketId]
    ON [dbo].[TreatmentEvents]([TicketId] ASC)
    INCLUDE([Id], [CompanyId], [DivisionId], [CustomerId], [VisitDate], [VisitStatus], [CreatedBy], [CreatedOn], [ModifiedBy], [TreatmentConfigId], [TreatmentId], [ProgramId], [CustomColorId], [Comment]);


GO
CREATE NONCLUSTERED INDEX [Index_Rep_ClubRating_2]
    ON [dbo].[TreatmentEvents]([TreatmentConfigId] ASC, [VisitDate] ASC, [VisitStatus] ASC)
    INCLUDE([TicketId]);


GO
CREATE NONCLUSTERED INDEX [IX_SMS]
    ON [dbo].[TreatmentEvents]([VisitStatus] ASC, [DivisionId] ASC, [VisitDate] ASC)
    INCLUDE([CustomerId], [CreatedOn]);


GO
CREATE NONCLUSTERED INDEX [IX_TREATMENTEVENTS_CRMREPORT]
    ON [dbo].[TreatmentEvents]([VisitStatus] ASC)
    INCLUDE([CompanyId], [VisitDate]);


GO
CREATE NONCLUSTERED INDEX [IX_CompanyId]
    ON [dbo].[TreatmentEvents]([CompanyId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ModifiedBy]
    ON [dbo].[TreatmentEvents]([ModifiedBy] ASC)
    INCLUDE([Id], [CompanyId], [DivisionId], [CustomerId], [VisitDate], [VisitStatus], [CreatedBy], [CreatedOn], [TicketId], [TreatmentConfigId], [TreatmentId], [ProgramId], [CustomColorId], [Comment]);


GO
CREATE NONCLUSTERED INDEX [IX_DivisionId]
    ON [dbo].[TreatmentEvents]([DivisionId] ASC)
    INCLUDE([Id], [CompanyId], [ModifiedBy], [CustomerId], [VisitDate], [VisitStatus], [CreatedBy], [CreatedOn], [TicketId], [TreatmentConfigId], [TreatmentId], [ProgramId], [CustomColorId], [Comment]);


GO
CREATE NONCLUSTERED INDEX [IX_CustomerId]
    ON [dbo].[TreatmentEvents]([CustomerId] ASC)
    INCLUDE([Id], [CompanyId], [ModifiedBy], [DivisionId], [VisitDate], [VisitStatus], [CreatedBy], [CreatedOn], [TicketId], [TreatmentConfigId], [TreatmentId], [ProgramId], [CustomColorId], [Comment]);

