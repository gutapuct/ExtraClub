CREATE TABLE [dbo].[EmployeePayments] (
    [Id]            UNIQUEIDENTIFIER CONSTRAINT [DF_EmployeePayments_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [CompanyId]     UNIQUEIDENTIFIER NOT NULL,
    [SpendingId]    UNIQUEIDENTIFIER NOT NULL,
    [SalarySheetId] UNIQUEIDENTIFIER NULL,
    [EmployeeId]    UNIQUEIDENTIFIER NOT NULL,
    [PaymentType]   SMALLINT         NOT NULL,
    [Amount]        MONEY            NOT NULL,
    [Period]        DATE             NOT NULL,
    [Log]           NVARCHAR (MAX)   NULL,
    [CreatedOn]     DATETIME         NULL,
    CONSTRAINT [PK_EmployeePayments] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_EmployeePayments_Employees] FOREIGN KEY ([EmployeeId]) REFERENCES [dbo].[Employees] ([Id]),
    CONSTRAINT [FK_EmployeePayments_SalarySheets] FOREIGN KEY ([SalarySheetId]) REFERENCES [dbo].[SalarySheets] ([Id])
);


GO
ALTER TABLE [dbo].[EmployeePayments] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

