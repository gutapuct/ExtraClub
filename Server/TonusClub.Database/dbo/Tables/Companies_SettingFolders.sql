CREATE TABLE [dbo].[Companies_SettingFolders] (
    [CompanyId]        UNIQUEIDENTIFIER NOT NULL,
    [SettingsFolderId] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_Companies_SettingFolders] PRIMARY KEY CLUSTERED ([CompanyId] ASC, [SettingsFolderId] ASC),
    CONSTRAINT [FK_Companies_SettingFolders_Companies] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Companies] ([CompanyId]),
    CONSTRAINT [FK_Companies_SettingFolders_SettingsFolders] FOREIGN KEY ([SettingsFolderId]) REFERENCES [dbo].[SettingsFolders] ([Id])
);


GO
ALTER TABLE [dbo].[Companies_SettingFolders] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

