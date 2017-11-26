CREATE TABLE [dbo].[TreatmentTypeGroups] (
    [Id]      UNIQUEIDENTIFIER CONSTRAINT [DF_TreatmentTypeGroup_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [Name]    NVARCHAR (MAX)   NOT NULL,
    [IsAvail] BIT              CONSTRAINT [DF_TreatmentTypeGroups_IsAvail] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_TreatmentTypeGroup] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
ALTER TABLE [dbo].[TreatmentTypeGroups] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

