CREATE TABLE [dbo].[CustomerCards] (
    [Id]                 UNIQUEIDENTIFIER CONSTRAINT [DF_ClientCards_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [CreatedBy]          UNIQUEIDENTIFIER NOT NULL,
    [CustomerId]         UNIQUEIDENTIFIER NOT NULL,
    [CardBarcode]        NVARCHAR (50)    NOT NULL,
    [EmitDate]           DATETIME         NOT NULL,
    [CustomerCardTypeId] UNIQUEIDENTIFIER NOT NULL,
    [Price]              MONEY            NOT NULL,
    [Discount]           MONEY            NOT NULL,
    [Comment]            NVARCHAR (MAX)   NULL,
    [CompanyId]          UNIQUEIDENTIFIER NOT NULL,
    [DivisionId]         UNIQUEIDENTIFIER NOT NULL,
    [IsActive]           BIT              CONSTRAINT [DF_CustomerCards_IsActive] DEFAULT ((1)) NOT NULL,
    [PmtTypeId]          INT              CONSTRAINT [DF_CustomerCards_PmtTypeId] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_ClientCards] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ClientCards_Customers] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customers] ([Id]),
    CONSTRAINT [FK_ClientCards_Users] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users] ([UserId]),
    CONSTRAINT [FK_CustomerCards_CustomerCardTypes] FOREIGN KEY ([CustomerCardTypeId]) REFERENCES [dbo].[CustomerCardTypes] ([Id]),
    CONSTRAINT [FK_CustomerCards_Divisions] FOREIGN KEY ([DivisionId]) REFERENCES [dbo].[Divisions] ([Id])
);


GO
ALTER TABLE [dbo].[CustomerCards] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);


GO
CREATE NONCLUSTERED INDEX [IX_CompnyId]
    ON [dbo].[CustomerCards]([CompanyId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CustomerId]
    ON [dbo].[CustomerCards]([CustomerId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DivisionId]
    ON [dbo].[CustomerCards]([DivisionId] ASC);

