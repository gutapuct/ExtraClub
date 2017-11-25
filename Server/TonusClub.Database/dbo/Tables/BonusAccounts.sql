CREATE TABLE [dbo].[BonusAccounts] (
    [Id]          UNIQUEIDENTIFIER CONSTRAINT [DF_BonusAccounts_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [CreatedBy]   UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn]   DATETIME         CONSTRAINT [DF_BonusAccounts_CreatedOn] DEFAULT (getdate()) NOT NULL,
    [CustomerId]  UNIQUEIDENTIFIER NOT NULL,
    [Amount]      MONEY            NOT NULL,
    [Description] NVARCHAR (500)   NOT NULL,
    [CompanyId]   UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_BonusAccounts] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_BonusAccounts_Customers] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customers] ([Id]),
    CONSTRAINT [FK_BonusAccounts_Users] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users] ([UserId])
);


GO
ALTER TABLE [dbo].[BonusAccounts] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);


GO
CREATE NONCLUSTERED INDEX [IX_CompanyId]
    ON [dbo].[BonusAccounts]([CompanyId] ASC);

