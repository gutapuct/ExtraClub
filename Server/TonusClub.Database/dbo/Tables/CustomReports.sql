CREATE TABLE [dbo].[CustomReports] (
    [Id]           UNIQUEIDENTIFIER CONSTRAINT [DF_CustomReports_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [CompanyId]    UNIQUEIDENTIFIER NULL,
    [CreatedBy]    UNIQUEIDENTIFIER NOT NULL,
    [Name]         NVARCHAR (256)   NOT NULL,
    [Comments]     NVARCHAR (MAX)   NULL,
    [CustomFields] NVARCHAR (MAX)   NULL,
    [XmlClause]    VARBINARY (MAX)  NOT NULL,
    [BaseTypeName] NVARCHAR (50)    NOT NULL,
    [IsFixed]      BIT              CONSTRAINT [DF_CustomReports_IsFixed] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_CustomReports] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
ALTER TABLE [dbo].[CustomReports] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

