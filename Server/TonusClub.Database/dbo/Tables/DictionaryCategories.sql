CREATE TABLE [dbo].[DictionaryCategories] (
    [Id]       UNIQUEIDENTIFIER CONSTRAINT [DF_DictionaryCategories_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [Name]     NVARCHAR (250)   NOT NULL,
    [ParentId] UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_DictionaryCategories] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_DictionaryCategories_DictionaryCategories] FOREIGN KEY ([ParentId]) REFERENCES [dbo].[DictionaryCategories] ([Id])
);


GO
ALTER TABLE [dbo].[DictionaryCategories] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

