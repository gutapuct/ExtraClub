CREATE TABLE [dbo].[Consignments] (
    [Id]                      UNIQUEIDENTIFIER CONSTRAINT [DF_Consignments_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [Number]                  INT              NOT NULL,
    [Date]                    SMALLDATETIME    NOT NULL,
    [ProviderId]              UNIQUEIDENTIFIER NULL,
    [DivisionId]              UNIQUEIDENTIFIER NULL,
    [Sdal]                    NVARCHAR (250)   NULL,
    [Prinal]                  NVARCHAR (250)   NULL,
    [Comment]                 NVARCHAR (500)   NULL,
    [CreatedBy]               UNIQUEIDENTIFIER NOT NULL,
    [CompanyId]               UNIQUEIDENTIFIER NOT NULL,
    [DocType]                 SMALLINT         CONSTRAINT [DF_Consignments_DocType] DEFAULT ((0)) NOT NULL,
    [IncomeNumber]            NVARCHAR (50)    NULL,
    [DestinationStorehouseId] UNIQUEIDENTIFIER NULL,
    [SourceStorehouseId]      UNIQUEIDENTIFIER NULL,
    [IsAsset]                 BIT              CONSTRAINT [DF_Consignments_IsAsset_1] DEFAULT ((0)) NOT NULL,
    [ProviderOrderId]         UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_Consignments] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Consignments_Companies] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Companies] ([CompanyId]),
    CONSTRAINT [FK_Consignments_Consignments] FOREIGN KEY ([ProviderOrderId]) REFERENCES [dbo].[Consignments] ([Id]),
    CONSTRAINT [FK_Consignments_Divisions] FOREIGN KEY ([DivisionId]) REFERENCES [dbo].[Divisions] ([Id]),
    CONSTRAINT [FK_Consignments_Storehouses] FOREIGN KEY ([DestinationStorehouseId]) REFERENCES [dbo].[Storehouses] ([Id]),
    CONSTRAINT [FK_Consignments_Storehouses1] FOREIGN KEY ([SourceStorehouseId]) REFERENCES [dbo].[Storehouses] ([Id]),
    CONSTRAINT [FK_Consignments_Users] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users] ([UserId])
);


GO
ALTER TABLE [dbo].[Consignments] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);


GO
CREATE NONCLUSTERED INDEX [IX_CompanyId]
    ON [dbo].[Consignments]([CompanyId] ASC);

