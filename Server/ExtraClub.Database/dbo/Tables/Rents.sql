CREATE TABLE [dbo].[Rents] (
    [Id]             UNIQUEIDENTIFIER CONSTRAINT [DF_Rents_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [CompanyId]      UNIQUEIDENTIFIER NOT NULL,
    [CustomerId]     UNIQUEIDENTIFIER NOT NULL,
    [CreatedBy]      UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn]      DATETIME         NOT NULL,
    [GoodId]         UNIQUEIDENTIFIER NOT NULL,
    [StorehouseId]   UNIQUEIDENTIFIER NOT NULL,
    [Price]          MONEY            NOT NULL,
    [ReturnDate]     DATETIME         NOT NULL,
    [ReturnById]     UNIQUEIDENTIFIER NULL,
    [FactReturnDate] DATETIME         NULL,
    [LostFine]       MONEY            NULL,
    [OverdueFine]    MONEY            NULL,
    [IsPayed]        BIT              CONSTRAINT [DF_Rents_IsPayed] DEFAULT ((0)) NOT NULL,
    [IsManualAmount] BIT              CONSTRAINT [DF_Rents_IsManualAmount] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Rents] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Rents_Companies] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Companies] ([CompanyId]),
    CONSTRAINT [FK_Rents_Customers] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customers] ([Id]),
    CONSTRAINT [FK_Rents_Goods] FOREIGN KEY ([GoodId]) REFERENCES [dbo].[Goods] ([GoodId]),
    CONSTRAINT [FK_Rents_Storehouses] FOREIGN KEY ([StorehouseId]) REFERENCES [dbo].[Storehouses] ([Id]),
    CONSTRAINT [FK_Rents_Users] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users] ([UserId]),
    CONSTRAINT [FK_Rents_Users1] FOREIGN KEY ([ReturnById]) REFERENCES [dbo].[Users] ([UserId])
);


GO
ALTER TABLE [dbo].[Rents] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

