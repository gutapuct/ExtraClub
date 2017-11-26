CREATE TABLE [dbo].[ReportInfos] (
    [Id]             UNIQUEIDENTIFIER CONSTRAINT [DF_ReportInfos_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [Name]           NVARCHAR (250)   NOT NULL,
    [Type]           INT              NOT NULL,
    [MethodInfo]     NVARCHAR (MAX)   NOT NULL,
    [ReportComments] NVARCHAR (MAX)   NULL,
    CONSTRAINT [PK_ReportInfos] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
ALTER TABLE [dbo].[ReportInfos] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

