CREATE TABLE [dbo].[Goods] (
    [GoodId]          UNIQUEIDENTIFIER NOT NULL,
    [CompanyId]       UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn]       DATETIME         NOT NULL,
    [ModifiedOn]      DATETIME         NULL,
    [UserId]          UNIQUEIDENTIFIER NOT NULL,
    [Name]            NVARCHAR (MAX)   NOT NULL,
    [UnitTypeId]      UNIQUEIDENTIFIER NULL,
    [IntAmount]       BIT              CONSTRAINT [DF_Goods_IntAmount] DEFAULT ((0)) NOT NULL,
    [ProductTypeId]   UNIQUEIDENTIFIER NULL,
    [Description]     NVARCHAR (MAX)   NULL,
    [ManufacturerId]  UNIQUEIDENTIFIER NULL,
    [GoodsCategoryId] UNIQUEIDENTIFIER NULL,
    [BarCode]         VARCHAR (50)     NULL,
    [FirebirdId]      INT              NULL,
    [IsVisible]       BIT              CONSTRAINT [DF_Goods_IsVisible] DEFAULT ((1)) NOT NULL,
    [IsOurs]          BIT              CONSTRAINT [DF_Goods_IsOurs] DEFAULT ((0)) NOT NULL,
    [Code1C]          NVARCHAR (64)    NULL,
    CONSTRAINT [PK_Goods] PRIMARY KEY CLUSTERED ([GoodId] ASC),
    CONSTRAINT [FK_CompanyGood] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Companies] ([CompanyId]),
    CONSTRAINT [FK_GoodsCategoryGood] FOREIGN KEY ([GoodsCategoryId]) REFERENCES [dbo].[GoodsCategories] ([Id]),
    CONSTRAINT [FK_ManufacturerGood] FOREIGN KEY ([ManufacturerId]) REFERENCES [dbo].[Manufacturers] ([ManufacturerId]),
    CONSTRAINT [FK_ProductTypeGood] FOREIGN KEY ([ProductTypeId]) REFERENCES [dbo].[ProductTypes] ([Id]),
    CONSTRAINT [FK_UnitTypeGood] FOREIGN KEY ([UnitTypeId]) REFERENCES [dbo].[UnitTypes] ([Id]),
    CONSTRAINT [FK_UserGood] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId])
);


GO
ALTER TABLE [dbo].[Goods] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);


GO
CREATE NONCLUSTERED INDEX [IX_FK_CompanyGood]
    ON [dbo].[Goods]([CompanyId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_FK_UserGood]
    ON [dbo].[Goods]([UserId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_FK_UnitTypeGood]
    ON [dbo].[Goods]([UnitTypeId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_FK_ProductTypeGood]
    ON [dbo].[Goods]([ProductTypeId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_FK_ManufacturerGood]
    ON [dbo].[Goods]([ManufacturerId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_FK_GoodsCategoryGood]
    ON [dbo].[Goods]([GoodsCategoryId] ASC);

