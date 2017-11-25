CREATE TABLE [dbo].[CumulativeDiscounts] (
    [Id]              UNIQUEIDENTIFIER CONSTRAINT [DF_CumulativeDiscounts_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [CompanyId]       UNIQUEIDENTIFIER NOT NULL,
    [IsCountDisc]     BIT              CONSTRAINT [DF_CumulativeDiscounts_IsCountDisc] DEFAULT ((0)) NOT NULL,
    [ValueFrom]       MONEY            NOT NULL,
    [ValueTo]         MONEY            NOT NULL,
    [DiscountPercent] MONEY            NOT NULL,
    [CreatedById]     UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn]       DATETIME         NOT NULL,
    CONSTRAINT [PK_CumulativeDiscounts] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
ALTER TABLE [dbo].[CumulativeDiscounts] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

