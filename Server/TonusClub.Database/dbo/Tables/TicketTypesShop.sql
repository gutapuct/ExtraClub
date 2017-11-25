CREATE TABLE [dbo].[TicketTypesShop] (
    [Id]           UNIQUEIDENTIFIER CONSTRAINT [DF_TicketTypesShop_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [TicketTypeId] UNIQUEIDENTIFIER NOT NULL,
    [CompanyId]    UNIQUEIDENTIFIER NOT NULL,
    [Price]        MONEY            NOT NULL,
    CONSTRAINT [PK_TicketTypesShop] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_TicketTypesShop_TicketTypes] FOREIGN KEY ([TicketTypeId]) REFERENCES [dbo].[TicketTypes] ([Id])
);


GO
ALTER TABLE [dbo].[TicketTypesShop] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

