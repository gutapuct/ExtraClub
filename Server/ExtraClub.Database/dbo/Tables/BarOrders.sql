CREATE TABLE [dbo].[BarOrders] (
    [Id]              UNIQUEIDENTIFIER CONSTRAINT [DF_BarOrders_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [CustomerId]      UNIQUEIDENTIFIER NOT NULL,
    [DivisionId]      UNIQUEIDENTIFIER NOT NULL,
    [PurchaseDate]    DATETIME         NOT NULL,
    [CreatedBy]       UNIQUEIDENTIFIER NOT NULL,
    [OrderNumber]     INT              NOT NULL,
    [CashPayment]     MONEY            CONSTRAINT [DF_BarOrders_CashPayment] DEFAULT ((0)) NOT NULL,
    [DepositPayment]  MONEY            CONSTRAINT [DF_BarOrders_DepositPayment] DEFAULT ((0)) NOT NULL,
    [CardPayment]     MONEY            CONSTRAINT [DF_BarOrders_CardPayment] DEFAULT ((0)) NOT NULL,
    [CardNumber]      NVARCHAR (20)    NULL,
    [CardAuth]        NVARCHAR (50)    NULL,
    [BonusPayment]    MONEY            NULL,
    [CompanyId]       UNIQUEIDENTIFIER NOT NULL,
    [Content]         VARBINARY (MAX)  NULL,
    [CertificateId]   UNIQUEIDENTIFIER NULL,
    [ProviderId]      UNIQUEIDENTIFIER NULL,
    [PaymentDate]     DATETIME         NULL,
    [PaymentComments] NVARCHAR (MAX)   NULL,
    [GoodActionId]    UNIQUEIDENTIFIER NULL,
    [SectionNumber]   INT              CONSTRAINT [DF_BarOrders_SectionNumber] DEFAULT ((1)) NOT NULL,
    [Kind1C]          INT              NULL,
    CONSTRAINT [PK_BarOrders] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_BarOrders_Certificates] FOREIGN KEY ([CertificateId]) REFERENCES [dbo].[Certificates] ([Id]),
    CONSTRAINT [FK_BarOrders_Customers] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customers] ([Id]),
    CONSTRAINT [FK_BarOrders_Divisions] FOREIGN KEY ([DivisionId]) REFERENCES [dbo].[Divisions] ([Id]),
    CONSTRAINT [FK_BarOrders_Kinds1C] FOREIGN KEY ([Kind1C]) REFERENCES [dbo].[Kinds1C] ([Id]),
    CONSTRAINT [FK_BarOrders_Providers] FOREIGN KEY ([ProviderId]) REFERENCES [dbo].[Providers] ([Id]),
    CONSTRAINT [FK_BarOrders_Users] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users] ([UserId])
);


GO
ALTER TABLE [dbo].[BarOrders] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);


GO
CREATE NONCLUSTERED INDEX [IX_BarOrders_CPP]
    ON [dbo].[BarOrders]([CompanyId] ASC, [PaymentDate] ASC, [ProviderId] ASC)
    INCLUDE([Id], [CustomerId], [DivisionId], [PurchaseDate], [CreatedBy], [OrderNumber], [CashPayment], [DepositPayment], [CardPayment], [CardNumber], [CardAuth], [BonusPayment], [Content], [CertificateId], [PaymentComments], [GoodActionId], [SectionNumber], [Kind1C]);


GO
CREATE NONCLUSTERED INDEX [IX_BarOrders_Anket]
    ON [dbo].[BarOrders]([DivisionId] ASC, [PurchaseDate] ASC)
    INCLUDE([CashPayment], [CardPayment]);


GO
CREATE NONCLUSTERED INDEX [IX_CompanyID]
    ON [dbo].[BarOrders]([CompanyId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CustomerId]
    ON [dbo].[BarOrders]([CustomerId] ASC);

GO
CREATE NONCLUSTERED INDEX IX_BarOrders_Opt1
	ON [dbo].[BarOrders] ([DivisionId],[DepositPayment])
	INCLUDE ([Id])

