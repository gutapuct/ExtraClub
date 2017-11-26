CREATE TABLE [dbo].[Providers] (
    [Id]                 UNIQUEIDENTIFIER CONSTRAINT [DF_Providers_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [Name]               NVARCHAR (250)   NOT NULL,
    [INN]                NVARCHAR (250)   NULL,
    [KPP]                NVARCHAR (250)   NULL,
    [FullName]           NVARCHAR (250)   NULL,
    [CorrAccount]        NVARCHAR (250)   NULL,
    [SettlementAccount]  NVARCHAR (250)   NULL,
    [Bank]               NVARCHAR (250)   NULL,
    [BIK]                NVARCHAR (250)   NULL,
    [OKPO]               NVARCHAR (250)   NULL,
    [OKONH]              NVARCHAR (250)   NULL,
    [Director]           NVARCHAR (250)   NULL,
    [LegalAddress]       NVARCHAR (250)   NULL,
    [RealAddress]        NVARCHAR (250)   NULL,
    [Transport]          NVARCHAR (250)   NULL,
    [Accountant]         NVARCHAR (250)   NULL,
    [Phone1]             NVARCHAR (250)   NULL,
    [Phone2]             NVARCHAR (250)   NULL,
    [Phone3]             NVARCHAR (250)   NULL,
    [Fax]                NVARCHAR (250)   NULL,
    [Email]              NVARCHAR (250)   NULL,
    [WebSite]            NVARCHAR (250)   NULL,
    [OrganizationTypeId] UNIQUEIDENTIFIER NULL,
    [WorkTime]           NVARCHAR (250)   NULL,
    [Comments]           NVARCHAR (250)   NULL,
    [CreatedBy]          UNIQUEIDENTIFIER NOT NULL,
    [CompanyId]          UNIQUEIDENTIFIER NOT NULL,
    [ContactPerson]      NVARCHAR (250)   NULL,
    [PostAddress]        NVARCHAR (250)   NULL,
    [Ogrn]               NVARCHAR (250)   NULL,
    [ProviderFolderId]   UNIQUEIDENTIFIER NULL,
    [IsVisible]          BIT              CONSTRAINT [DF_Providers_IsVisible] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_Providers] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Providers_Companies] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Companies] ([CompanyId]),
    CONSTRAINT [FK_Providers_ProviderFolders] FOREIGN KEY ([ProviderFolderId]) REFERENCES [dbo].[ProviderFolders] ([Id]),
    CONSTRAINT [FK_Providers_Providers] FOREIGN KEY ([OrganizationTypeId]) REFERENCES [dbo].[OrganizationTypes] ([Id]),
    CONSTRAINT [FK_Providers_Users] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users] ([UserId])
);


GO
ALTER TABLE [dbo].[Providers] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

