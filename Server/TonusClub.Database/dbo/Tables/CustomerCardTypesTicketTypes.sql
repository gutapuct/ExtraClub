CREATE TABLE [dbo].[CustomerCardTypesTicketTypes] (
    [CustomerCardTypeId] UNIQUEIDENTIFIER NOT NULL,
    [TicketTypeId]       UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_CustomerCardTypesTicketTypes] PRIMARY KEY CLUSTERED ([CustomerCardTypeId] ASC, [TicketTypeId] ASC),
    CONSTRAINT [FK_CustomerCardTypesTicketTypes_CustomerCardTypes] FOREIGN KEY ([CustomerCardTypeId]) REFERENCES [dbo].[CustomerCardTypes] ([Id]),
    CONSTRAINT [FK_CustomerCardTypesTicketTypes_TicketTypes] FOREIGN KEY ([TicketTypeId]) REFERENCES [dbo].[TicketTypes] ([Id])
);


GO
ALTER TABLE [dbo].[CustomerCardTypesTicketTypes] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

