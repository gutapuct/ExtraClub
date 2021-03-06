﻿CREATE TABLE [dbo].[News] (
    [Id]        UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn] DATETIME         NOT NULL,
    [Subject]   NVARCHAR (300)   NOT NULL,
    [Message]   NVARCHAR (MAX)   NOT NULL,
    [CreatedBy] UNIQUEIDENTIFIER NOT NULL,
    [Url]       NVARCHAR (512)   NULL,
    [UrlTitle]  NVARCHAR (512)   NULL,
    CONSTRAINT [PK_News] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
ALTER TABLE [dbo].[News] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

