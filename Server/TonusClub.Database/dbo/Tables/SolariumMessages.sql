CREATE TABLE [dbo].[SolariumMessages] (
    [Id]         UNIQUEIDENTIFIER CONSTRAINT [DF_SolariumMessages_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [MinMinutes] INT              NOT NULL,
    [Message]    NVARCHAR (256)   NULL,
    CONSTRAINT [PK_SolariumMessages] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
ALTER TABLE [dbo].[SolariumMessages] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

