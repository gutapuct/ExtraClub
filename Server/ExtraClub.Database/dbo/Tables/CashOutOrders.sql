CREATE TABLE [dbo].[CashOutOrders] (
    [Id]             UNIQUEIDENTIFIER CONSTRAINT [DF_CashOutOrders_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [CompanyId]      UNIQUEIDENTIFIER NOT NULL,
    [DivisionId]     UNIQUEIDENTIFIER NOT NULL,
    [Number]         INT              NOT NULL,
    [CreatedOn]      DATETIME         NOT NULL,
    [CreatedById]    UNIQUEIDENTIFIER NOT NULL,
    [ReceivedById]   UNIQUEIDENTIFIER NOT NULL,
    [Amount]         MONEY            NOT NULL,
    [Reason]         NVARCHAR (4000)  NOT NULL,
    [Debet]          NVARCHAR (4000)  NOT NULL,
    [Responsible]    NVARCHAR (4000)  NOT NULL,
    [ReceivedByText] NVARCHAR (4000)  NULL,
    CONSTRAINT [PK_CashOutOrders] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_CashOutOrders_Companies] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Companies] ([CompanyId]),
    CONSTRAINT [FK_CashOutOrders_Divisions] FOREIGN KEY ([DivisionId]) REFERENCES [dbo].[Divisions] ([Id]),
    CONSTRAINT [FK_CashOutOrders_Users] FOREIGN KEY ([CreatedById]) REFERENCES [dbo].[Users] ([UserId])
);


GO
ALTER TABLE [dbo].[CashOutOrders] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

