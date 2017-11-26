CREATE TABLE [dbo].[AnketTickets] (
    [Id]           UNIQUEIDENTIFIER CONSTRAINT [DF_AnketTickets_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [CompanyId]    UNIQUEIDENTIFIER NOT NULL,
    [AnketId]      UNIQUEIDENTIFIER NOT NULL,
    [TicketTypeId] UNIQUEIDENTIFIER NOT NULL,
    [Amount]       INT              NOT NULL,
    CONSTRAINT [PK_AnketTickets] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AnketTickets_Ankets] FOREIGN KEY ([AnketId]) REFERENCES [dbo].[Ankets] ([Id]),
    CONSTRAINT [FK_AnketTickets_TicketTypes] FOREIGN KEY ([TicketTypeId]) REFERENCES [dbo].[TicketTypes] ([Id])
);


GO
ALTER TABLE [dbo].[AnketTickets] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

