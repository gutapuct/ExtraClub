CREATE TABLE [dbo].[DictionaryInfo] (
    [Id]                   UNIQUEIDENTIFIER CONSTRAINT [DF_DictionaryInfo_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [DisplayName]          NVARCHAR (250)   NOT NULL,
    [EntitySetName]        NVARCHAR (250)   NOT NULL,
    [IdRow]                NVARCHAR (250)   NOT NULL,
    [DisplayRow]           NVARCHAR (250)   NOT NULL,
    [DictionaryCategoryId] UNIQUEIDENTIFIER NULL,
    [PermissionLevel]      INT              CONSTRAINT [DF_DictionaryInfo_PermissionLevel] DEFAULT ((0)) NOT NULL,
    [EntityTypeName]       NVARCHAR (250)   NOT NULL,
    [AvailRow]             NVARCHAR (250)   NULL,
    [DisplayNameEn]        NVARCHAR (250)   NULL,
    [DisplayRowEn]         NVARCHAR (250)   NULL,
    CONSTRAINT [PK_DictionaryInfo] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_DictionaryInfo_DictionaryCategories] FOREIGN KEY ([DictionaryCategoryId]) REFERENCES [dbo].[DictionaryCategories] ([Id]),
    CONSTRAINT [IX_DictionaryInfo] UNIQUE NONCLUSTERED ([EntitySetName] ASC)
);


GO
ALTER TABLE [dbo].[DictionaryInfo] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

