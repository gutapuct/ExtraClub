CREATE TABLE [dbo].[SavedReports] (
    [Id]                         UNIQUEIDENTIFIER CONSTRAINT [DF_SavedReports_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [CompanyId]                  UNIQUEIDENTIFIER NOT NULL,
    [ReportKey]                  NVARCHAR (256)   NOT NULL,
    [Name]                       NVARCHAR (512)   NOT NULL,
    [SerializedParametersValues] VARBINARY (MAX)  NOT NULL,
    [CreatedBy]                  UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_SavedReports] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_SavedReports_Users] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users] ([UserId])
);


GO
ALTER TABLE [dbo].[SavedReports] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

