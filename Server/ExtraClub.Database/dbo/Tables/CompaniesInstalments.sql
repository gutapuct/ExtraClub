CREATE TABLE [dbo].[CompaniesInstalments] (
    [CompanyId]    UNIQUEIDENTIFIER NOT NULL,
    [InstalmentId] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_CompaniesInstalments] PRIMARY KEY CLUSTERED ([CompanyId] ASC, [InstalmentId] ASC),
    CONSTRAINT [FK_CompaniesInstalments_Companies] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Companies] ([CompanyId]),
    CONSTRAINT [FK_CompaniesInstalments_Instalments] FOREIGN KEY ([InstalmentId]) REFERENCES [dbo].[Instalments] ([Id])
);


GO
ALTER TABLE [dbo].[CompaniesInstalments] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

