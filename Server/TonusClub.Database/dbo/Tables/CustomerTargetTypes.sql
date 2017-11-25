CREATE TABLE [dbo].[CustomerTargetTypes] (
    [Id]      UNIQUEIDENTIFIER CONSTRAINT [DF_CustomerTargetType_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [Name]    NVARCHAR (256)   NOT NULL,
    [IsAvail] BIT              CONSTRAINT [DF_CustomerTargetTypes_IsAvail] DEFAULT ((1)) NOT NULL,
    [NameEn]  NVARCHAR (256)   NULL,
    CONSTRAINT [PK_CustomerTargetType] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
ALTER TABLE [dbo].[CustomerTargetTypes] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

