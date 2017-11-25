CREATE TABLE [dbo].[GoodSales] (
    [Id]           UNIQUEIDENTIFIER CONSTRAINT [DF_GoodSales_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [GoodId]       UNIQUEIDENTIFIER NOT NULL,
    [Amount]       FLOAT (53)       NOT NULL,
    [PriceMoney]   MONEY            NULL,
    [PriceBonus]   MONEY            NULL,
    [BarOrderId]   UNIQUEIDENTIFIER NOT NULL,
    [CompanyId]    UNIQUEIDENTIFIER NOT NULL,
    [StorehouseId] UNIQUEIDENTIFIER NOT NULL,
    [ReturnDate]   DATETIME         NULL,
    [ReturnById]   UNIQUEIDENTIFIER NULL,
    [Discount]     MONEY            NULL,
    CONSTRAINT [PK_GoodSales] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_GoodSales_BarOrders] FOREIGN KEY ([BarOrderId]) REFERENCES [dbo].[BarOrders] ([Id]),
    CONSTRAINT [FK_GoodSales_Goods] FOREIGN KEY ([GoodId]) REFERENCES [dbo].[Goods] ([GoodId]),
    CONSTRAINT [FK_GoodSales_Storehouses] FOREIGN KEY ([StorehouseId]) REFERENCES [dbo].[Storehouses] ([Id]),
    CONSTRAINT [FK_GoodSales_Users] FOREIGN KEY ([ReturnById]) REFERENCES [dbo].[Users] ([UserId])
);


GO
ALTER TABLE [dbo].[GoodSales] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);


GO
CREATE NONCLUSTERED INDEX [IX_CompanyId]
    ON [dbo].[GoodSales]([CompanyId] ASC);

