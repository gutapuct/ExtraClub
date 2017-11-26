CREATE TABLE [dbo].[DivisionFinances] (
    [Id]           UNIQUEIDENTIFIER CONSTRAINT [DF_DivisionFinances_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [CompanyId]    UNIQUEIDENTIFIER NOT NULL,
    [DivisionId]   UNIQUEIDENTIFIER NOT NULL,
    [Period]       DATE             NOT NULL,
    [CashLeft]     MONEY            NOT NULL,
    [Unsent]       MONEY            NOT NULL,
    [Advances]     MONEY            NOT NULL,
    [TerminalLoan] MONEY            NOT NULL,
    [Accum]        MONEY            CONSTRAINT [DF_DivisionFinances_Accum] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_DivisionFinances] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_DivisionFinances_Companies] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Companies] ([CompanyId]),
    CONSTRAINT [FK_DivisionFinances_Divisions] FOREIGN KEY ([DivisionId]) REFERENCES [dbo].[Divisions] ([Id])
);


GO
ALTER TABLE [dbo].[DivisionFinances] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

