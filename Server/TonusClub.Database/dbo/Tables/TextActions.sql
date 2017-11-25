CREATE TABLE [dbo].[TextActions] (
    [Id]               UNIQUEIDENTIFIER CONSTRAINT [DF_TextActions_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [CompanyId]        UNIQUEIDENTIFIER NOT NULL,
    [CreatedBy]        UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn]        DATETIME         NOT NULL,
    [ActionText]       NVARCHAR (MAX)   NULL,
    [StartDate]        DATE             NOT NULL,
    [FinishDate]       DATE             NOT NULL,
    [SettingsFolderId] UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_TextActions] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_TextActions_Companies] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Companies] ([CompanyId]),
    CONSTRAINT [FK_TextActions_Users] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users] ([UserId])
);


GO
ALTER TABLE [dbo].[TextActions] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

