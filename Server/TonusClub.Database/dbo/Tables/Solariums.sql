CREATE TABLE [dbo].[Solariums] (
    [Id]                UNIQUEIDENTIFIER CONSTRAINT [DF_Solariums_Id] DEFAULT (newid()) NOT NULL,
    [Name]              NVARCHAR (250)   NULL,
    [CompanyId]         UNIQUEIDENTIFIER NOT NULL,
    [DivisionId]        UNIQUEIDENTIFIER NOT NULL,
    [CreatedBy]         UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn]         DATETIME         NOT NULL,
    [IsActive]          BIT              CONSTRAINT [DF_Solariums_IsActive] DEFAULT ((1)) NOT NULL,
    [Comment]           NVARCHAR (512)   NULL,
    [DogNumber]         NVARCHAR (MAX)   NULL,
    [SerialNumber]      NVARCHAR (MAX)   NULL,
    [Delivery]          NVARCHAR (MAX)   NULL,
    [GuaranteeExp]      NVARCHAR (MAX)   NULL,
    [UseExp]            NVARCHAR (MAX)   NULL,
    [LampsExpires]      DATETIME         NULL,
    [MaintenaceTime]    INT              CONSTRAINT [DF_Solariums_MaintenaceTime] DEFAULT ((3)) NOT NULL,
    [SettingsFolderId]  UNIQUEIDENTIFIER NULL,
    [LapsResource]      INT              CONSTRAINT [DF_Solariums_LapsResource] DEFAULT ((500)) NOT NULL,
    [Color]             NVARCHAR (MAX)   NULL,
    [Size]              NVARCHAR (MAX)   NULL,
    [Model]             NVARCHAR (MAX)   NULL,
    [MinutePrice]       MONEY            CONSTRAINT [DF_Solariums_Price] DEFAULT ((0)) NOT NULL,
    [TicketMinutePrice] MONEY            CONSTRAINT [DF_Solariums_TicketPrice] DEFAULT ((1)) NOT NULL,
    [Code1C]            NVARCHAR (64)    NULL,
    CONSTRAINT [PK_Solariums] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Solariums_Companies] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Companies] ([CompanyId]),
    CONSTRAINT [FK_Solariums_Divisions] FOREIGN KEY ([DivisionId]) REFERENCES [dbo].[Divisions] ([Id]),
    CONSTRAINT [FK_Solariums_Users] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users] ([UserId])
);


GO
ALTER TABLE [dbo].[Solariums] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

