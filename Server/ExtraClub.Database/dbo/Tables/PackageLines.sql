CREATE TABLE [dbo].[PackageLines] (
    [Id]        UNIQUEIDENTIFIER CONSTRAINT [DF_PackageLines_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [CompanyId] UNIQUEIDENTIFIER NOT NULL,
    [PackageId] UNIQUEIDENTIFIER NOT NULL,
    [GoodId]    UNIQUEIDENTIFIER NOT NULL,
    [Amount]    INT              NOT NULL,
    CONSTRAINT [PK_PackageLines] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_PackageLines_Packages] FOREIGN KEY ([PackageId]) REFERENCES [dbo].[Packages] ([Id])
);

