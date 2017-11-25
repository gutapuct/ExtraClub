CREATE TABLE [dbo].[BarDiscounts] (
    [Id]              UNIQUEIDENTIFIER CONSTRAINT [DF_BarDiscounts_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [CompanyId]       UNIQUEIDENTIFIER NOT NULL,
    [ValueFrom]       MONEY            NOT NULL,
    [ValueTo]         MONEY            NOT NULL,
    [DiscountPercent] MONEY            NOT NULL,
    [CreatedById]     UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn]       DATETIME         NOT NULL,
    CONSTRAINT [PK_BarDiscounts] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
ALTER TABLE [dbo].[BarDiscounts] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

