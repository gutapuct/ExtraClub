CREATE TABLE [dbo].[DepositOuts] (
    [Id]          UNIQUEIDENTIFIER CONSTRAINT [DF_DepositOut_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [CompanyId]   UNIQUEIDENTIFIER NOT NULL,
    [CustomerId]  UNIQUEIDENTIFIER NOT NULL,
    [CreatedBy]   UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn]   DATETIME         NOT NULL,
    [Amount]      MONEY            NOT NULL,
    [ProcessedBy] UNIQUEIDENTIFIER NULL,
    [ProcessedOn] DATETIME         NULL,
    [Comment]     NVARCHAR (512)   NULL,
    CONSTRAINT [PK_DepositOut] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_DepositOut_Companies] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Companies] ([CompanyId]),
    CONSTRAINT [FK_DepositOut_Customers] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customers] ([Id]),
    CONSTRAINT [FK_DepositOut_Users] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users] ([UserId]),
    CONSTRAINT [FK_DepositOut_Users1] FOREIGN KEY ([ProcessedBy]) REFERENCES [dbo].[Users] ([UserId])
);


GO
ALTER TABLE [dbo].[DepositOuts] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

