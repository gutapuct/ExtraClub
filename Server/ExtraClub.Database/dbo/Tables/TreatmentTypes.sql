CREATE TABLE [dbo].[TreatmentTypes] (
    [Id]                   UNIQUEIDENTIFIER CONSTRAINT [DF_Treatments_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [Name]                 NVARCHAR (250)   NOT NULL,
    [AllowsMultiple]       BIT              CONSTRAINT [DF_TreatmentTypes_AllowsMultiple] DEFAULT ((0)) NOT NULL,
    [Duration]             INT              NOT NULL,
    [TreatmentTypeGroupId] UNIQUEIDENTIFIER NULL,
    [IsActive]             BIT              CONSTRAINT [DF_TreatmentTypes_IsActive] DEFAULT ((1)) NOT NULL,
    [NameEn]               NVARCHAR (256)   NULL,
    CONSTRAINT [PK_Treatments] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_TreatmentTypes_TreatmentTypeGroups] FOREIGN KEY ([TreatmentTypeGroupId]) REFERENCES [dbo].[TreatmentTypeGroups] ([Id])
);


GO
ALTER TABLE [dbo].[TreatmentTypes] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

