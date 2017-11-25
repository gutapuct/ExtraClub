CREATE TABLE [dbo].[CashInOrders] (
    [Id]           UNIQUEIDENTIFIER CONSTRAINT [DF_CashInOrders_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [CompanyId]    UNIQUEIDENTIFIER NOT NULL,
    [DivisionId]   UNIQUEIDENTIFIER NOT NULL,
    [Number]       INT              NOT NULL,
    [CreatedOn]    DATETIME         NOT NULL,
    [CreatedById]  UNIQUEIDENTIFIER NOT NULL,
    [ReceivedById] UNIQUEIDENTIFIER NOT NULL,
    [Amount]       MONEY            NOT NULL,
    [Reason]       NVARCHAR (4000)  NOT NULL,
    [Debet]        NVARCHAR (4000)  NOT NULL,
    CONSTRAINT [PK_CashInOrders] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_CashInOrders_Companies] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Companies] ([CompanyId]),
    CONSTRAINT [FK_CashInOrders_Divisions] FOREIGN KEY ([DivisionId]) REFERENCES [dbo].[Divisions] ([Id]),
    CONSTRAINT [FK_CashInOrders_Users] FOREIGN KEY ([CreatedById]) REFERENCES [dbo].[Users] ([UserId]),
    CONSTRAINT [FK_CashInOrders_Users1] FOREIGN KEY ([ReceivedById]) REFERENCES [dbo].[Users] ([UserId])
);


GO
ALTER TABLE [dbo].[CashInOrders] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

