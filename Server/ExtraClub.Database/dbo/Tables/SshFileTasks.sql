﻿CREATE TABLE [dbo].[SshFileTasks] (
    [Id]     UNIQUEIDENTIFIER ROWGUIDCOL NOT NULL,
    [FileId] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_SshFileTasks] PRIMARY KEY CLUSTERED ([Id] ASC)
);

