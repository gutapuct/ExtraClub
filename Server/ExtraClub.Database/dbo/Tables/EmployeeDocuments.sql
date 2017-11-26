CREATE TABLE [dbo].[EmployeeDocuments] (
    [Id]          UNIQUEIDENTIFIER CONSTRAINT [DF_EmployeeDocuments_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [CompanyId]   UNIQUEIDENTIFIER NOT NULL,
    [Number]      INT              NOT NULL,
    [DocType]     INT              NOT NULL,
    [ReferenceId] UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn]   DATETIME         NOT NULL,
    CONSTRAINT [PK_EmployeeDocuments] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_EmployeeDocuments_Companies] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Companies] ([CompanyId])
);


GO
ALTER TABLE [dbo].[EmployeeDocuments] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

