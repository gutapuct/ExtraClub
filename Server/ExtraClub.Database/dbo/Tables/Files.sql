CREATE TABLE [dbo].[Files] (
    [Id]         UNIQUEIDENTIFIER CONSTRAINT [DF_Files_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [CompanyId]  UNIQUEIDENTIFIER NULL,
    [DivisionId] UNIQUEIDENTIFIER NULL,
    [Filename]   NVARCHAR (MAX)   NOT NULL,
    [CreatedOn]  DATETIME         NOT NULL,
    [Data]       VARBINARY (MAX)  NOT NULL,
    [Category]   INT              NULL,
    [Parameter]  UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_Files] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Files_Divisions] FOREIGN KEY ([DivisionId]) REFERENCES [dbo].[Divisions] ([Id])
);


GO
ALTER TABLE [dbo].[Files] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

