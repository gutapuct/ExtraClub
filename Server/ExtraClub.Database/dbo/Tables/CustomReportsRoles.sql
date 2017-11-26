CREATE TABLE [dbo].[CustomReportsRoles] (
    [CustomReportId] UNIQUEIDENTIFIER NOT NULL,
    [RoleId]         UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_CustomReportsRoles] PRIMARY KEY CLUSTERED ([CustomReportId] ASC, [RoleId] ASC),
    CONSTRAINT [FK_CustomReportsRoles_CustomReports] FOREIGN KEY ([CustomReportId]) REFERENCES [dbo].[CustomReports] ([Id]),
    CONSTRAINT [FK_CustomReportsRoles_Roles] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Roles] ([RoleId])
);


GO
ALTER TABLE [dbo].[CustomReportsRoles] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

