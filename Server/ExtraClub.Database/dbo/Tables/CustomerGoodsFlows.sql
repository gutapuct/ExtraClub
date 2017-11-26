CREATE TABLE [dbo].[CustomerGoodsFlows] (
    [Id]           UNIQUEIDENTIFIER CONSTRAINT [DF_CustomerGoodsFlows_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [CompanyId]    UNIQUEIDENTIFIER NOT NULL,
    [CustomerId]   UNIQUEIDENTIFIER NOT NULL,
    [GoodId]       UNIQUEIDENTIFIER NOT NULL,
    [Amount]       INT              NOT NULL,
    [Description]  NVARCHAR (500)   NOT NULL,
    [CreatedOn]    DATETIME         NOT NULL,
    [CreatedById]  UNIQUEIDENTIFIER NOT NULL,
    [DivisionId]   UNIQUEIDENTIFIER NOT NULL,
    [StorehouseId] UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_CustomerGoodsFlows] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_CustomerGoodsFlows_Customers] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customers] ([Id]),
    CONSTRAINT [FK_CustomerGoodsFlows_Goods] FOREIGN KEY ([GoodId]) REFERENCES [dbo].[Goods] ([GoodId])
);

