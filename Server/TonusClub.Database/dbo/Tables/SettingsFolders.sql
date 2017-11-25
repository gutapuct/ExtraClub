CREATE TABLE [dbo].[SettingsFolders] (
    [Id]             UNIQUEIDENTIFIER CONSTRAINT [DF_SettingsFolders_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [Name]           NVARCHAR (250)   NOT NULL,
    [ParentFolderId] UNIQUEIDENTIFIER NULL,
    [CategoryId]     INT              NOT NULL,
    CONSTRAINT [PK_SettingsFolders] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_SettingsFolders_SettingsFolders] FOREIGN KEY ([ParentFolderId]) REFERENCES [dbo].[SettingsFolders] ([Id])
);


GO
ALTER TABLE [dbo].[SettingsFolders] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

