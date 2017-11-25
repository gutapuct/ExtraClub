CREATE TABLE [dbo].[SshFiles] (
    [Id]            UNIQUEIDENTIFIER CONSTRAINT [DF_SshFiles_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [Path]          NVARCHAR (1024)  NOT NULL,
    [Filename]      NVARCHAR (256)   NOT NULL,
    [Length]        BIGINT           NOT NULL,
    [ModifiedDate]  DATETIME         NOT NULL,
    [AvailableTill] DATE             NULL,
    CONSTRAINT [PK_SshFiles] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
ALTER TABLE [dbo].[SshFiles] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

