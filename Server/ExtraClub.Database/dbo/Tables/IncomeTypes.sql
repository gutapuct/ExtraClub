CREATE TABLE [dbo].[IncomeTypes] (
    [Id]         UNIQUEIDENTIFIER CONSTRAINT [DF_IncomeTypes_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [CompanyId]  UNIQUEIDENTIFIER NULL,
    [DivisionId] UNIQUEIDENTIFIER NULL,
    [IsCommon]   BIT              CONSTRAINT [DF_IncomeTypes_IsCommon] DEFAULT ((0)) NOT NULL,
    [Name]       NVARCHAR (256)   NOT NULL,
    [IsDeleted]  BIT              CONSTRAINT [DF_IncomeTypes_IsActive] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_IncomeTypes] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_IncomeTypes_Companies] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Companies] ([CompanyId]),
    CONSTRAINT [FK_IncomeTypes_Divisions] FOREIGN KEY ([DivisionId]) REFERENCES [dbo].[Divisions] ([Id])
);


GO
ALTER TABLE [dbo].[IncomeTypes] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

