CREATE TABLE [dbo].[SalarySchemeCoefficients] (
    [Id]             UNIQUEIDENTIFIER CONSTRAINT [DF_SalarySchemeCoefficients_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [CompanyId]      UNIQUEIDENTIFIER NOT NULL,
    [SalarySchemeId] UNIQUEIDENTIFIER NOT NULL,
    [CoeffTypeId]    INT              NOT NULL,
    [Money1]         MONEY            NULL,
    [Guid1]          UNIQUEIDENTIFIER NULL,
    [Int1]           INT              NULL,
    [Int2]           INT              NULL,
    [TimeSpan1]      TIME (7)         NULL,
    [TimeSpan2]      TIME (7)         NULL,
    CONSTRAINT [PK_SalarySchemeCoefficients] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_SalarySchemeCoefficients_SalaryScheme] FOREIGN KEY ([SalarySchemeId]) REFERENCES [dbo].[SalaryScheme] ([Id])
);


GO
ALTER TABLE [dbo].[SalarySchemeCoefficients] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

