CREATE TABLE [dbo].[SalaryRateTables] (
    [Id]                        UNIQUEIDENTIFIER CONSTRAINT [DF_SalaryRateTables_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [CompanyId]                 UNIQUEIDENTIFIER NOT NULL,
    [SalarySchemeCoefficientId] UNIQUEIDENTIFIER NOT NULL,
    [FromValue]                 MONEY            NULL,
    [ToValue]                   MONEY            NULL,
    [Result]                    MONEY            NOT NULL,
    CONSTRAINT [PK_SalaryRateTables] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_SalaryRateTables_SalarySchemeCoefficients] FOREIGN KEY ([SalarySchemeCoefficientId]) REFERENCES [dbo].[SalarySchemeCoefficients] ([Id])
);


GO
ALTER TABLE [dbo].[SalaryRateTables] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

