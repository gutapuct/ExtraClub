CREATE TABLE [dbo].[EmployeeCategories] (
    [Id]              UNIQUEIDENTIFIER CONSTRAINT [DF_EmployeeCategories_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [Name]            NVARCHAR (250)   NOT NULL,
    [Description]     NVARCHAR (512)   NULL,
    [SalaryMulti]     MONEY            CONSTRAINT [DF_Table_2_SalaryMulty] DEFAULT ((1)) NOT NULL,
    [IsPupilContract] BIT              NOT NULL,
    [CompanyId]       UNIQUEIDENTIFIER NOT NULL,
    [IsActive]        BIT              CONSTRAINT [DF_EmployeeCategories_IsActive_1] DEFAULT ((1)) NOT NULL,
    [CreatedOn]       DATETIME         CONSTRAINT [DF_EmployeeCategories_CreatedOn_1] DEFAULT (getdate()) NOT NULL,
    [PrevCategoryId]  UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_EmployeeCategories] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_EmployeeCategories_Companies] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Companies] ([CompanyId]),
    CONSTRAINT [FK_EmployeeCategories_EmployeeCategories] FOREIGN KEY ([PrevCategoryId]) REFERENCES [dbo].[EmployeeCategories] ([Id])
);


GO
ALTER TABLE [dbo].[EmployeeCategories] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

