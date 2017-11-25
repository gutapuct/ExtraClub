CREATE TABLE [dbo].[Permissions] (
    [PermissionId]          UNIQUEIDENTIFIER NOT NULL,
    [PermissionKey]         VARCHAR (50)     NOT NULL,
    [PermissionName]        NVARCHAR (64)    NULL,
    [PermissionDescription] NVARCHAR (512)   NULL,
    [CreatedOn]             DATETIME         NOT NULL,
    [ModifiedOn]            DATETIME         NULL,
    [ParentPermissionId]    UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_Permissions] PRIMARY KEY CLUSTERED ([PermissionId] ASC),
    CONSTRAINT [FK_Permissions_Permissions] FOREIGN KEY ([ParentPermissionId]) REFERENCES [dbo].[Permissions] ([PermissionId])
);


GO
ALTER TABLE [dbo].[Permissions] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

