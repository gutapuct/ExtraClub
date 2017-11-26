CREATE TABLE [dbo].[SalarySheets] (
    [Id]          UNIQUEIDENTIFIER CONSTRAINT [DF_SalarySheets_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [CompanyId]   UNIQUEIDENTIFIER NOT NULL,
    [DivisionId]  UNIQUEIDENTIFIER NOT NULL,
    [CreatedBy]   UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn]   DATETIME         NOT NULL,
    [PeriodStart] DATE             NOT NULL,
    CONSTRAINT [PK_SalarySheets] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_SalarySheets_Divisions] FOREIGN KEY ([DivisionId]) REFERENCES [dbo].[Divisions] ([Id]),
    CONSTRAINT [FK_SalarySheets_Users] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users] ([UserId])
);


GO
ALTER TABLE [dbo].[SalarySheets] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

