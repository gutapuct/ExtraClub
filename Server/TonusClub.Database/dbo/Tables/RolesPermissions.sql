CREATE TABLE [dbo].[RolesPermissions] (
    [Permissions_PermissionId] UNIQUEIDENTIFIER NOT NULL,
    [Roles_RoleId]             UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_RolesPermissions] PRIMARY KEY NONCLUSTERED ([Permissions_PermissionId] ASC, [Roles_RoleId] ASC),
    CONSTRAINT [FK_RolesPermissions_Permissions] FOREIGN KEY ([Permissions_PermissionId]) REFERENCES [dbo].[Permissions] ([PermissionId]),
    CONSTRAINT [FK_RolesPermissions_Roles] FOREIGN KEY ([Roles_RoleId]) REFERENCES [dbo].[Roles] ([RoleId])
);


GO
ALTER TABLE [dbo].[RolesPermissions] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);


GO
CREATE NONCLUSTERED INDEX [IX_FK_RolesPermissions_Roles]
    ON [dbo].[RolesPermissions]([Roles_RoleId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_USERSROLESPERMISSIONS]
    ON [dbo].[RolesPermissions]([Roles_RoleId] ASC)
    INCLUDE([Permissions_PermissionId]);

