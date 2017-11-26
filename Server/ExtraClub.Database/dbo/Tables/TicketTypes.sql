CREATE TABLE [dbo].[TicketTypes] (
    [Id]               UNIQUEIDENTIFIER CONSTRAINT [DF_TicketTypes_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [Name]             NVARCHAR (250)   NOT NULL,
    [Units]            MONEY            NOT NULL,
    [GuestUnits]       MONEY            NOT NULL,
    [Price]            MONEY            NOT NULL,
    [Comments]         NVARCHAR (512)   NULL,
    [CreatedBy]        UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn]        DATETIME         CONSTRAINT [DF_TicketTypes_CreatedOn] DEFAULT (getdate()) NOT NULL,
    [ValidTo]          DATETIME         NOT NULL,
    [Length]           INT              CONSTRAINT [DF_TicketTypes_Length] DEFAULT ((0)) NOT NULL,
    [IsActive]         BIT              CONSTRAINT [DF_TicketTypes_IsActive] DEFAULT ((1)) NOT NULL,
    [AutoActivate]     INT              NULL,
    [IsVisit]          BIT              CONSTRAINT [DF_TicketTypes_IsVisit] DEFAULT ((0)) NOT NULL,
    [IsGuest]          BIT              CONSTRAINT [DF_TicketTypes_IsGuest] DEFAULT ((0)) NOT NULL,
    [VisitStart]       NCHAR (4)        NULL,
    [VisitEnd]         NCHAR (4)        NULL,
    [MaxFreezeDays]    INT              CONSTRAINT [DF_TicketTypes_MaxFreezeLength] DEFAULT ((0)) NOT NULL,
    [Bonus]            MONEY            CONSTRAINT [DF_TicketTypes_Bonus] DEFAULT ((0)) NOT NULL,
    [FreezePriceCoeff] FLOAT (53)       CONSTRAINT [DF_TicketTypes_FreezePriceCoeff] DEFAULT ((1)) NOT NULL,
    [SolariumMinutes]  INT              CONSTRAINT [DF_TicketTypes_SolariumMinutes] DEFAULT ((0)) NOT NULL,
    [IsAction]         BIT              CONSTRAINT [DF_TicketTypes_IsAction] DEFAULT ((0)) NOT NULL,
    [SettingsFolderId] UNIQUEIDENTIFIER NULL,
    [HasTestdrive]     BIT              CONSTRAINT [DF_TicketTypes_HasTestdrive] DEFAULT ((1)) NOT NULL,
    [Code1C]           NVARCHAR (64)    NULL,
    [IsSmart]          BIT              CONSTRAINT [DF_TicketTypes_IsSmart] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_TicketTypes] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
ALTER TABLE [dbo].[TicketTypes] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

