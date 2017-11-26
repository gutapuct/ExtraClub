CREATE TABLE [dbo].[CompanySettingsFolders] (
    [Id]             UNIQUEIDENTIFIER CONSTRAINT [DF_CompanySettingsFolders_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [Name]           NVARCHAR (250)   NOT NULL,
    [CompanyId]      UNIQUEIDENTIFIER NOT NULL,
    [DivisionId]     UNIQUEIDENTIFIER NULL,
    [ParentFolderId] UNIQUEIDENTIFIER NULL,
    [CategoryId]     INT              NOT NULL,
    CONSTRAINT [PK_CompanySettingsFolders] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_CompanySettingsFolders_CompanySettingsFolders] FOREIGN KEY ([ParentFolderId]) REFERENCES [dbo].[CompanySettingsFolders] ([Id])
);

