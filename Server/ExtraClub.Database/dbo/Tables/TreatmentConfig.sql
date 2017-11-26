CREATE TABLE [dbo].[TreatmentConfig] (
    [Id]               UNIQUEIDENTIFIER CONSTRAINT [DF_TreatmentConfig_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [Name]             NVARCHAR (MAX)   NOT NULL,
    [TreatmentTypeId]  UNIQUEIDENTIFIER NOT NULL,
    [LengthCoeff]      INT              NOT NULL,
    [Price]            MONEY            NOT NULL,
    [IsActive]         BIT              CONSTRAINT [DF_TreatmentConfig_IsActive] DEFAULT ((1)) NOT NULL,
    [NameEn]           NVARCHAR (256)   NULL,
    [SettingsFolderId] UNIQUEIDENTIFIER NULL,
    [DisableAges]      NVARCHAR (500)   NULL,
    CONSTRAINT [PK_TreatmentConfig] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_TreatmentConfig_TreatmentTypes] FOREIGN KEY ([TreatmentTypeId]) REFERENCES [dbo].[TreatmentTypes] ([Id])
);


GO
ALTER TABLE [dbo].[TreatmentConfig] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

