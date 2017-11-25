CREATE TABLE [dbo].[Packages] (
    [Id]        UNIQUEIDENTIFIER ROWGUIDCOL NOT NULL,
    [CompanyId] UNIQUEIDENTIFIER NOT NULL,
    [Name]      NVARCHAR (500)   NOT NULL,
    [IsActive]  BIT              CONSTRAINT [DF_Packages_IsActive] DEFAULT ((1)) NOT NULL,
    [Price]     MONEY            NOT NULL,
    CONSTRAINT [PK_Packages] PRIMARY KEY CLUSTERED ([Id] ASC)
);

