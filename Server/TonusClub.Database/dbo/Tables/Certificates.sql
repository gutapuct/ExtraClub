CREATE TABLE [dbo].[Certificates] (
    [Id]          UNIQUEIDENTIFIER CONSTRAINT [DF_Certificates_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [CompanyId]   UNIQUEIDENTIFIER NOT NULL,
    [DivisionId]  UNIQUEIDENTIFIER NOT NULL,
    [CreatedBy]   UNIQUEIDENTIFIER NOT NULL,
    [BarCode]     NVARCHAR (50)    NOT NULL,
    [Amount]      MONEY            NOT NULL,
    [UsedOrderId] UNIQUEIDENTIFIER NULL,
    [PriceMoney]  MONEY            NULL,
    [PriceBonus]  INT              NULL,
    [BuyerId]     UNIQUEIDENTIFIER NULL,
    [SellDate]    DATETIME         NULL,
    [CreatedOn]   DATETIME         NOT NULL,
    [CategoryId]  UNIQUEIDENTIFIER NULL,
    [Name]        NVARCHAR (255)   NULL,
    [IsBonusSell] BIT              CONSTRAINT [DF_Certificates_IsBonusSell_1] DEFAULT ((0)) NOT NULL,
    [SellOrderId] UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_Certificates] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Certificates_BarOrders] FOREIGN KEY ([UsedOrderId]) REFERENCES [dbo].[BarOrders] ([Id]),
    CONSTRAINT [FK_Certificates_BarOrders1] FOREIGN KEY ([SellOrderId]) REFERENCES [dbo].[BarOrders] ([Id]),
    CONSTRAINT [FK_Certificates_Companies] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Companies] ([CompanyId]),
    CONSTRAINT [FK_Certificates_Customers] FOREIGN KEY ([BuyerId]) REFERENCES [dbo].[Customers] ([Id]),
    CONSTRAINT [FK_Certificates_Divisions] FOREIGN KEY ([DivisionId]) REFERENCES [dbo].[Divisions] ([Id]),
    CONSTRAINT [FK_Certificates_GoodsCategories] FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[GoodsCategories] ([Id]),
    CONSTRAINT [FK_Certificates_Users] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users] ([UserId])
);


GO
ALTER TABLE [dbo].[Certificates] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

