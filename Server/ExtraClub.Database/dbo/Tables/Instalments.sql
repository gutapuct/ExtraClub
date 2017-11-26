CREATE TABLE [dbo].[Instalments] (
    [Id]                    UNIQUEIDENTIFIER CONSTRAINT [DF_Instalments_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [IsActive]              BIT              CONSTRAINT [DF_Instalments_IsActive] DEFAULT ((1)) NOT NULL,
    [Name]                  NVARCHAR (250)   NULL,
    [ContribPercent]        DECIMAL (18, 2)  NULL,
    [ContribAmount]         MONEY            NULL,
    [Length]                INT              NOT NULL,
    [AvailableUnitsPercent] DECIMAL (18, 2)  NOT NULL,
    [CreatedBy]             UNIQUEIDENTIFIER NOT NULL,
    [SettingsFolderId]      UNIQUEIDENTIFIER NULL,
    [SecondPercent]         MONEY            NULL,
    [SecondLength]          INT              NULL,
    CONSTRAINT [PK_Instalments] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
ALTER TABLE [dbo].[Instalments] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

