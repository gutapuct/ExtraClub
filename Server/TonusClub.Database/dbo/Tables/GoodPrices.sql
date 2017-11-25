CREATE TABLE [dbo].[GoodPrices] (
    [Id]            UNIQUEIDENTIFIER CONSTRAINT [DF_GoodPrices_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [GoodId]        UNIQUEIDENTIFIER NOT NULL,
    [DivisionId]    UNIQUEIDENTIFIER NOT NULL,
    [Date]          DATETIME         NOT NULL,
    [CommonPrice]   MONEY            NOT NULL,
    [EmployeePrice] MONEY            NULL,
    [BonusPrice]    MONEY            NULL,
    [CreatedBy]     UNIQUEIDENTIFIER NOT NULL,
    [CompanyId]     UNIQUEIDENTIFIER NOT NULL,
    [InPricelist]   BIT              CONSTRAINT [DF_GoodPrices_InPricelist] DEFAULT ((1)) NOT NULL,
    [Comments]      NVARCHAR (250)   NULL,
    [RentPrice]     MONEY            NULL,
    [RentFine]      MONEY            NULL,
    CONSTRAINT [PK_GoodPrices] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_GoodPrices_Companies] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Companies] ([CompanyId]),
    CONSTRAINT [FK_GoodPrices_Divisions] FOREIGN KEY ([DivisionId]) REFERENCES [dbo].[Divisions] ([Id]),
    CONSTRAINT [FK_GoodPrices_Goods] FOREIGN KEY ([GoodId]) REFERENCES [dbo].[Goods] ([GoodId]),
    CONSTRAINT [FK_GoodPrices_Users] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users] ([UserId])
);


GO
ALTER TABLE [dbo].[GoodPrices] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);


GO
CREATE NONCLUSTERED INDEX [IX_CompanyId]
    ON [dbo].[GoodPrices]([CompanyId] ASC);

