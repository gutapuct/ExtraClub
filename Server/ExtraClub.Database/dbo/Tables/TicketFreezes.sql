CREATE TABLE [dbo].[TicketFreezes] (
    [Id]                   UNIQUEIDENTIFIER CONSTRAINT [DF_TicketFreezes_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [TicketId]             UNIQUEIDENTIFIER NOT NULL,
    [StartDate]            DATE             NOT NULL,
    [FinishDate]           DATE             NOT NULL,
    [TicketFreezeReasonId] UNIQUEIDENTIFIER NOT NULL,
    [Comment]              NVARCHAR (250)   NULL,
    [CreatedBy]            UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn]            DATETIME         CONSTRAINT [DF_TicketFreezes_CreatedOn] DEFAULT (getdate()) NOT NULL,
    [CompanyId]            UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_TicketFreezes] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_TicketFreezes_TicketFreezeReasons] FOREIGN KEY ([TicketFreezeReasonId]) REFERENCES [dbo].[TicketFreezeReasons] ([Id]),
    CONSTRAINT [FK_TicketFreezes_Tickets] FOREIGN KEY ([TicketId]) REFERENCES [dbo].[Tickets] ([Id]),
    CONSTRAINT [FK_TicketFreezes_Users] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users] ([UserId])
);


GO
ALTER TABLE [dbo].[TicketFreezes] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);


GO
CREATE NONCLUSTERED INDEX [IX_TicketFreezes_TicketId]
    ON [dbo].[TicketFreezes]([TicketId] ASC)
    INCLUDE([Id], [StartDate], [FinishDate], [TicketFreezeReasonId], [Comment], [CreatedBy], [CreatedOn], [CompanyId]);


GO
CREATE NONCLUSTERED INDEX [IX_CompanyId]
    ON [dbo].[TicketFreezes]([CompanyId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TIcketFreezeReasonId]
    ON [dbo].[TicketFreezes]([TicketFreezeReasonId] ASC, [TicketId] ASC, [CreatedOn] ASC);

