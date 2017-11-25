CREATE TABLE [dbo].[EmployeeVisits] (
    [Id]         UNIQUEIDENTIFIER CONSTRAINT [DF_EmployeeVisits_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [CompanyId]  UNIQUEIDENTIFIER NOT NULL,
    [EmployeeId] UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn]  DATETIME         NOT NULL,
    [IsIncome]   BIT              CONSTRAINT [DF_EmployeeVisits_IsIncome] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_EmployeeVisits] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_EmployeeVisits_Companies] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Companies] ([CompanyId]),
    CONSTRAINT [FK_EmployeeVisits_Employees] FOREIGN KEY ([EmployeeId]) REFERENCES [dbo].[Employees] ([Id])
);


GO
ALTER TABLE [dbo].[EmployeeVisits] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);


GO
CREATE NONCLUSTERED INDEX [IX_CompanyId]
    ON [dbo].[EmployeeVisits]([CompanyId] ASC);

