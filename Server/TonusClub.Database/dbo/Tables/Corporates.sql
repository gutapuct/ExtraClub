CREATE TABLE [dbo].[Corporates] (
    [Id]               UNIQUEIDENTIFIER CONSTRAINT [DF_Corporates_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [CompanyId]        UNIQUEIDENTIFIER NOT NULL,
    [Name]             NVARCHAR (250)   NOT NULL,
    [IsAvail]          BIT              CONSTRAINT [DF_Corporates_IsAvail] DEFAULT ((1)) NOT NULL,
    [SettingsFolderId] UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_Corporates] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Corporates_Companies] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Companies] ([CompanyId])
);


GO
ALTER TABLE [dbo].[Corporates] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

