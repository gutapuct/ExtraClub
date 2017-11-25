CREATE TABLE [dbo].[Emails] (
    [ID]          UNIQUEIDENTIFIER NOT NULL,
    [TicketID]    UNIQUEIDENTIFIER NOT NULL,
    [Destination] VARCHAR (100)    NOT NULL,
    [Subject]     VARCHAR (200)    NOT NULL,
    [Body]        VARCHAR (MAX)    NOT NULL,
    CONSTRAINT [PK_Notifications] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_Tickets_Emails] FOREIGN KEY ([TicketID]) REFERENCES [dbo].[Tickets] ([Id])
);

