CREATE TABLE [dbo].[CompanyFinances] (
    [Id]          UNIQUEIDENTIFIER CONSTRAINT [DF_CompanyFinances_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [CompanyId]   UNIQUEIDENTIFIER NOT NULL,
    [Period]      DATE             NOT NULL,
    [AccountLeft] MONEY            NOT NULL,
    CONSTRAINT [PK_CompanyFinances] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_CompanyFinances_Companies] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Companies] ([CompanyId])
);


GO
ALTER TABLE [dbo].[CompanyFinances] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

