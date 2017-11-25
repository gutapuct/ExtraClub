CREATE TABLE [dbo].[EmailItems]
(
	[Id] UNIQUEIDENTIFIER NOT NULL, 
    [Address] NVARCHAR(255) NOT NULL, 
    [Subject] NVARCHAR(1024) NOT NULL, 
    [Message] NVARCHAR(MAX) NOT NULL, 
    [CreatedOn] DATETIME NOT NULL, 
    [SendFrom] DATETIME NOT NULL, 
    [SentOn] DATETIME NULL, 
    [LastErrorMessage] NVARCHAR(MAX) NULL,
    CONSTRAINT [PK_EmailItems] PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO

CREATE NONCLUSTERED INDEX IX_EmailItems_Opt1
ON [dbo].[EmailItems] ([SentOn],[SendFrom])
