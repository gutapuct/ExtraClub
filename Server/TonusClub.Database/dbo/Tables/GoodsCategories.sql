CREATE TABLE [dbo].[GoodsCategories] (
    [Id]                UNIQUEIDENTIFIER NOT NULL,
    [GoodsCategoryName] NVARCHAR (MAX)   NOT NULL,
    [ModifiedOn]        DATETIME         NULL,
    [CreatedOn]         DATETIME         NOT NULL,
    [AuthorId]          UNIQUEIDENTIFIER NOT NULL,
    [FirebirdId]        INT              NULL,
    [IsAvail]           BIT              CONSTRAINT [DF_GoodsCategories_IsAvail] DEFAULT ((1)) NOT NULL,
    [NameEn]            NVARCHAR (256)   NULL,
    CONSTRAINT [PK_GoodsCategories] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
ALTER TABLE [dbo].[GoodsCategories] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);


GO
CREATE NONCLUSTERED INDEX [IX_FK_UserGoodsCategory]
    ON [dbo].[GoodsCategories]([AuthorId] ASC);

