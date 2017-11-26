CREATE TABLE [dbo].[UsersRoles] (
    [Roles_RoleId] UNIQUEIDENTIFIER NOT NULL,
    [Users_UserId] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_UsersRoles] PRIMARY KEY NONCLUSTERED ([Roles_RoleId] ASC, [Users_UserId] ASC),
    CONSTRAINT [FK_UsersRoles_Roles] FOREIGN KEY ([Roles_RoleId]) REFERENCES [dbo].[Roles] ([RoleId]),
    CONSTRAINT [FK_UsersRoles_Users] FOREIGN KEY ([Users_UserId]) REFERENCES [dbo].[Users] ([UserId])
);


GO
ALTER TABLE [dbo].[UsersRoles] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);


GO
CREATE NONCLUSTERED INDEX [IX_FK_UsersRoles_Users]
    ON [dbo].[UsersRoles]([Users_UserId] ASC);

