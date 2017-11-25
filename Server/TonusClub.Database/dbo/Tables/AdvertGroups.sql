CREATE TABLE [dbo].[AdvertGroups] (
    [Id]       UNIQUEIDENTIFIER CONSTRAINT [DF_AdvertGroups_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [Name]     NVARCHAR (MAX)   NOT NULL,
    [IsActive] BIT              NOT NULL,
    [NameEn]   NVARCHAR (256)   NULL,
    CONSTRAINT [PK_AdvertGroups] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
ALTER TABLE [dbo].[AdvertGroups] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

