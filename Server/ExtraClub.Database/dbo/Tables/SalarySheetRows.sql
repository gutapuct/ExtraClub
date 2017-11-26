CREATE TABLE [dbo].[SalarySheetRows] (
    [Id]            UNIQUEIDENTIFIER CONSTRAINT [DF_SalarySheetRows_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [CompanyId]     UNIQUEIDENTIFIER NOT NULL,
    [SalarySheetId] UNIQUEIDENTIFIER NOT NULL,
    [EmployeeId]    UNIQUEIDENTIFIER NOT NULL,
    [Salary]        MONEY            NOT NULL,
    [Bonus]         MONEY            NOT NULL,
    [NDFL]          MONEY            NOT NULL,
    [Ved10]         MONEY            NOT NULL,
    [Ved25]         MONEY            NOT NULL,
    [Log]           NVARCHAR (MAX)   NOT NULL,
    CONSTRAINT [PK_SalarySheetRows] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_SalarySheetRows_Employees] FOREIGN KEY ([EmployeeId]) REFERENCES [dbo].[Employees] ([Id]),
    CONSTRAINT [FK_SalarySheetRows_SalarySheets] FOREIGN KEY ([SalarySheetId]) REFERENCES [dbo].[SalarySheets] ([Id])
);


GO
ALTER TABLE [dbo].[SalarySheetRows] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

