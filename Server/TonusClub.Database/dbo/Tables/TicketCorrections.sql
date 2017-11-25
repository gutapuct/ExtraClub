CREATE TABLE [dbo].[TicketCorrections] (
    [Id]           UNIQUEIDENTIFIER CONSTRAINT [DF_TicketCorrections_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [CompanyId]    UNIQUEIDENTIFIER NOT NULL,
    [TicketId]     UNIQUEIDENTIFIER NOT NULL,
    [UserId]       UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn]    DATETIME         NOT NULL,
    [PropertyName] NVARCHAR (256)   NOT NULL,
    [OldValue]     NVARCHAR (256)   NOT NULL,
    [NewValue]     NVARCHAR (256)   NOT NULL,
    [Comment]      NVARCHAR (256)   NOT NULL,
    CONSTRAINT [PK_TicketCorrections] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_TicketCorrections_Tickets] FOREIGN KEY ([TicketId]) REFERENCES [dbo].[Tickets] ([Id]),
    CONSTRAINT [FK_TicketCorrections_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId])
);


GO
ALTER TABLE [dbo].[TicketCorrections] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);


GO
CREATE NONCLUSTERED INDEX [IX_CompanyId]
    ON [dbo].[TicketCorrections]([CompanyId] ASC);

