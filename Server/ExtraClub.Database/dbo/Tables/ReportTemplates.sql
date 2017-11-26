CREATE TABLE [dbo].[ReportTemplates] (
    [Id]            UNIQUEIDENTIFIER CONSTRAINT [DF_ReportTemplates_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [Name]          NVARCHAR (50)    NOT NULL,
    [HtmlText]      NVARCHAR (MAX)   NULL,
    [Description]   NVARCHAR (MAX)   NULL,
    [DisplayName]   NVARCHAR (250)   NULL,
    [DisplayNameEn] NVARCHAR (256)   NULL,
    [HtmlTextEn]    NVARCHAR (MAX)   NULL,
    [CompanyId]     UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_ReportTemplates] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
ALTER TABLE [dbo].[ReportTemplates] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);


GO
CREATE NONCLUSTERED INDEX [IX_CompanyId]
    ON [dbo].[ReportTemplates]([CompanyId] ASC);

