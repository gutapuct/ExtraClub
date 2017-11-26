CREATE TABLE [dbo].[ProviderPayments] (
    [Id]          UNIQUEIDENTIFIER CONSTRAINT [DF_ProviderPayments_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [ProviderId]  UNIQUEIDENTIFIER NOT NULL,
    [Date]        SMALLDATETIME    NOT NULL,
    [Number]      INT              NULL,
    [Amount]      MONEY            NOT NULL,
    [Comment]     NVARCHAR (250)   NULL,
    [AuthorId]    UNIQUEIDENTIFIER NOT NULL,
    [CompanyId]   UNIQUEIDENTIFIER NOT NULL,
    [FirebirdId]  INT              NULL,
    [OrderId]     UNIQUEIDENTIFIER NOT NULL,
    [PaymentType] NVARCHAR (50)    NULL,
    CONSTRAINT [PK_ProviderPayments] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ProviderPayments_Companies] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Companies] ([CompanyId]),
    CONSTRAINT [FK_ProviderPayments_Consignments] FOREIGN KEY ([OrderId]) REFERENCES [dbo].[Consignments] ([Id]),
    CONSTRAINT [FK_ProviderPayments_Users] FOREIGN KEY ([AuthorId]) REFERENCES [dbo].[Users] ([UserId])
);


GO
ALTER TABLE [dbo].[ProviderPayments] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

