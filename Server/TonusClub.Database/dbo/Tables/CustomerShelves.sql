CREATE TABLE [dbo].[CustomerShelves] (
    [Id]          UNIQUEIDENTIFIER CONSTRAINT [DF_CustomerShelves_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [CompanyId]   UNIQUEIDENTIFIER NOT NULL,
    [DivisionId]  UNIQUEIDENTIFIER NOT NULL,
    [CustomerId]  UNIQUEIDENTIFIER NOT NULL,
    [ShelfNumber] INT              NOT NULL,
    [CreatedOn]   DATETIME         NOT NULL,
    [CreatedBy]   UNIQUEIDENTIFIER NOT NULL,
    [ReturnOn]    DATETIME         NULL,
    [ReturnBy]    UNIQUEIDENTIFIER NULL,
    [Penalty]     INT              NULL,
    [IsSafe]      BIT              CONSTRAINT [DF_CustomerShelves_IsSafe_1] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_CustomerShelves] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_CustomerShelves_Companies] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Companies] ([CompanyId]),
    CONSTRAINT [FK_CustomerShelves_Customers] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customers] ([Id]),
    CONSTRAINT [FK_CustomerShelves_Divisions] FOREIGN KEY ([DivisionId]) REFERENCES [dbo].[Divisions] ([Id]),
    CONSTRAINT [FK_CustomerShelves_Users] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users] ([UserId]),
    CONSTRAINT [FK_CustomerShelves_Users1] FOREIGN KEY ([ReturnBy]) REFERENCES [dbo].[Users] ([UserId])
);


GO
ALTER TABLE [dbo].[CustomerShelves] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);


GO
CREATE NONCLUSTERED INDEX [IX_CompanyId]
    ON [dbo].[CustomerShelves]([CompanyId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedBy]
    ON [dbo].[CustomerShelves]([CreatedBy] ASC)
    INCLUDE([Id], [CompanyId], [DivisionId], [CustomerId], [ShelfNumber], [CreatedOn], [ReturnOn], [ReturnBy], [Penalty], [IsSafe]);

