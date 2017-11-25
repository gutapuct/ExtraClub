CREATE TABLE [dbo].[CustomerCardTypes] (
    [Id]                UNIQUEIDENTIFIER CONSTRAINT [DF_CustomerCardTypes_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [Name]              NVARCHAR (250)   NOT NULL,
    [Price]             MONEY            NOT NULL,
    [IsActive]          BIT              NOT NULL,
    [Bonus]             MONEY            NOT NULL,
    [Color]             NVARCHAR (250)   NULL,
    [CreatedBy]         UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn]         DATETIME         NOT NULL,
    [IsGuest]           BIT              CONSTRAINT [DF_CustomerCardTypes_IsGuest] DEFAULT ((0)) NOT NULL,
    [IsVisit]           BIT              CONSTRAINT [DF_CustomerCardTypes_IsVisit] DEFAULT ((0)) NOT NULL,
    [LostPenalty]       MONEY            CONSTRAINT [DF_CustomerCardTypes_LostPenalty] DEFAULT ((0)) NOT NULL,
    [BonusPercent]      MONEY            CONSTRAINT [DF_CustomerCardTypes_BonusPercent] DEFAULT ((0)) NOT NULL,
    [Description]       NVARCHAR (1000)  NULL,
    [FreezePriceCoeff]  FLOAT (53)       CONSTRAINT [DF_CustomerCardTypes_FreezePriceCoeff] DEFAULT ((1)) NOT NULL,
    [ChildrenCost]      MONEY            CONSTRAINT [DF_CustomerCardTypes_ChildrenCost] DEFAULT ((0)) NOT NULL,
    [SettingsFolderId]  UNIQUEIDENTIFIER NULL,
    [Code1C]            NVARCHAR (64)    NULL,
    [DiscountBar]       MONEY            CONSTRAINT [DF_CustomerCardTypes_DiscountBar] DEFAULT ((0)) NOT NULL,
    [GiveBonusForCards] BIT              CONSTRAINT [DF_CustomerCardTypes_GiveBonusForCards] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_CustomerCardTypes] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
ALTER TABLE [dbo].[CustomerCardTypes] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

