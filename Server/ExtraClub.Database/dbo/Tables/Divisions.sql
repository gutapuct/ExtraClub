CREATE TABLE [dbo].[Divisions] (
    [Id]                      UNIQUEIDENTIFIER CONSTRAINT [DF_Divisions_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [Name]                    NVARCHAR (250)   NOT NULL,
    [CreatedBy]               UNIQUEIDENTIFIER NOT NULL,
    [CompanyId]               UNIQUEIDENTIFIER NOT NULL,
    [Address]                 NVARCHAR (250)   NULL,
    [ShelvesRepository]       NVARCHAR (2500)  NULL,
    [SafesRepository]         NVARCHAR (2500)  NULL,
    [SolariumPrice]           MONEY            CONSTRAINT [DF_Divisions_SolriumPrice] DEFAULT ((10)) NOT NULL,
    [MinSolarium]             INT              CONSTRAINT [DF_Divisions_MinSolarium_1] DEFAULT ((1)) NOT NULL,
    [MaxSolarium]             INT              CONSTRAINT [DF_Divisions_MaxSolarium_1] DEFAULT ((20)) NOT NULL,
    [OpenTime]                TIME (7)         NULL,
    [CloseTime]               TIME (7)         NULL,
    [RCashRegister]           BIT              CONSTRAINT [DF_Divisions_RCashRegister_1] DEFAULT ((1)) NOT NULL,
    [RBankCards]              BIT              CONSTRAINT [DF_Divisions_RBankCards_1] DEFAULT ((1)) NOT NULL,
    [RCashless]               BIT              CONSTRAINT [DF_Divisions_RCashless_1] DEFAULT ((1)) NOT NULL,
    [RReceiptOnBank]          BIT              CONSTRAINT [DF_Divisions_RReceiptOnBank_1] DEFAULT ((1)) NOT NULL,
    [BankCardReturnComission] MONEY            CONSTRAINT [DF_Divisions_BankCardReturnComission] DEFAULT ((0.02)) NOT NULL,
    [WorkGraphNotifyDay]      INT              CONSTRAINT [DF_Divisions_WorkGraphNotifyDay] DEFAULT ((15)) NOT NULL,
    [InventoryDay]            INT              CONSTRAINT [DF_Divisions_InventoryDay] DEFAULT ((1)) NOT NULL,
    [IsAvail]                 BIT              CONSTRAINT [DF_Divisions_IsAvail] DEFAULT ((1)) NOT NULL,
    [SyncFailEmail]           NVARCHAR (2500)  NULL,
    [ConcessionNumber]        NVARCHAR (MAX)   NULL,
    [ConcessionDate]          DATE             NULL,
    [CityName]                NVARCHAR (250)   NULL,
    [HasSubway]               BIT              CONSTRAINT [DF_Divisions_HasSubway] DEFAULT ((1)) NOT NULL,
    [Index]                   NVARCHAR (MAX)   NULL,
    [Street]                  NVARCHAR (MAX)   NULL,
    [Building]                NVARCHAR (MAX)   NULL,
    [OpenTime2]               TIME (7)         NULL,
    [CloseTime2]              TIME (7)         NULL,
    [OpenTime3]               TIME (7)         NULL,
    [CloseTime3]              TIME (7)         NULL,
    [OpenTime4]               TIME (7)         NULL,
    [CloseTime4]              TIME (7)         NULL,
    [OpenTime5]               TIME (7)         NULL,
    [CloseTime5]              TIME (7)         NULL,
    [OpenTime6]               TIME (7)         NULL,
    [CloseTime6]              TIME (7)         NULL,
    [OpenTime7]               TIME (7)         NULL,
    [CloseTime7]              TIME (7)         NULL,
    [OpenDate]                DATE             NULL,
    [PresellDate]             DATE             NULL,
    [HasChildren]             BIT              CONSTRAINT [DF_Divisions_HasChildren] DEFAULT ((1)) NOT NULL,
    [CustomerBirthdayDays]    INT              CONSTRAINT [DF_Divisions_CustomerBirthdayDays] DEFAULT ((1)) NOT NULL,
    [Act]                     NVARCHAR (MAX)   NULL,
    [GeoCoordinates]          NVARCHAR (30)    NULL,
    [ShowOnSite]              BIT              CONSTRAINT [DF_Divisions_ShowOnSite] DEFAULT ((0)) NOT NULL,
    [SitePath]                NVARCHAR (80)    NULL,
    [MaxCash]                 MONEY            CONSTRAINT [DF_Divisions_MaxCash] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Divisions] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Divisions_Companies] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Companies] ([CompanyId]),
    CONSTRAINT [FK_Divisions_Users] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users] ([UserId])
);


GO
ALTER TABLE [dbo].[Divisions] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'1=по кассе, 2=БСО', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Divisions', @level2type = N'COLUMN', @level2name = N'RCashRegister';

