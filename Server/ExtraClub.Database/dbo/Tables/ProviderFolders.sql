CREATE TABLE [dbo].[ProviderFolders] (
    [Id]             UNIQUEIDENTIFIER CONSTRAINT [DF_ProviderFolders_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [CompanyId]      UNIQUEIDENTIFIER NOT NULL,
    [CreatedBy]      UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn]      DATETIME         NOT NULL,
    [Name]           NVARCHAR (250)   NOT NULL,
    [ParentFolderId] UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_ProviderFolders] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ProviderFolders_Companies] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Companies] ([CompanyId]),
    CONSTRAINT [FK_ProviderFolders_ProviderFolders] FOREIGN KEY ([ParentFolderId]) REFERENCES [dbo].[ProviderFolders] ([Id]),
    CONSTRAINT [FK_ProviderFolders_Users] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users] ([UserId])
);


GO
ALTER TABLE [dbo].[ProviderFolders] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

