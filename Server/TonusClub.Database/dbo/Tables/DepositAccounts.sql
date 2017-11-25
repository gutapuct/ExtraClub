CREATE TABLE [dbo].[DepositAccounts] (
    [Id]          UNIQUEIDENTIFIER CONSTRAINT [DF_DepositAccounts_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [CreatedBy]   UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn]   DATETIME         CONSTRAINT [DF_DepositAccounts_CreatedOn] DEFAULT (getdate()) NOT NULL,
    [CustomerId]  UNIQUEIDENTIFIER NOT NULL,
    [Amount]      MONEY            NOT NULL,
    [Description] NVARCHAR (500)   NOT NULL,
    [CompanyId]   UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_DepositAccounts] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_DepositAccounts_Customers] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customers] ([Id]),
    CONSTRAINT [FK_DepositAccounts_Users] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users] ([UserId])
);


GO
ALTER TABLE [dbo].[DepositAccounts] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);


GO
CREATE NONCLUSTERED INDEX [IX_CompanyId]
    ON [dbo].[DepositAccounts]([CompanyId] ASC);

