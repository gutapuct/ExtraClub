CREATE TABLE [dbo].[Storehouses] (
    [Id]               UNIQUEIDENTIFIER CONSTRAINT [DF_Storehouses_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [CompanyId]        UNIQUEIDENTIFIER NOT NULL,
    [DivisionId]       UNIQUEIDENTIFIER NOT NULL,
    [Address]          NVARCHAR (250)   NULL,
    [Name]             NVARCHAR (250)   NULL,
    [Responsible]      NVARCHAR (250)   NULL,
    [BarSale]          BIT              CONSTRAINT [DF_Storehouses_BarSale] DEFAULT ((1)) NOT NULL,
    [SettingsFolderId] UNIQUEIDENTIFIER NULL,
    [IsActive]         BIT              CONSTRAINT [DF_Storehouses_IsActive] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_Storehouses] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Storehouses_Companies] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Companies] ([CompanyId]),
    CONSTRAINT [FK_Storehouses_Divisions] FOREIGN KEY ([DivisionId]) REFERENCES [dbo].[Divisions] ([Id])
);


GO
ALTER TABLE [dbo].[Storehouses] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

