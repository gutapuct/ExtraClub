CREATE TABLE [dbo].[Incomes] (
    [Id]           UNIQUEIDENTIFIER CONSTRAINT [DF_Incomes_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [CompanyId]    UNIQUEIDENTIFIER NOT NULL,
    [DivisionId]   UNIQUEIDENTIFIER NULL,
    [CreatedOn]    DATETIME         NOT NULL,
    [CreatedBy]    UNIQUEIDENTIFIER NOT NULL,
    [Number]       INT              NOT NULL,
    [Name]         NVARCHAR (MAX)   NULL,
    [IncomeTypeId] UNIQUEIDENTIFIER NOT NULL,
    [Amount]       MONEY            NOT NULL,
    [PaymentType]  NVARCHAR (512)   NULL,
    [IsFinAction]  BIT              CONSTRAINT [DF_Incomes_IsFinAction] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Incomes] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Incomes_Companies] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Companies] ([CompanyId]),
    CONSTRAINT [FK_Incomes_Divisions] FOREIGN KEY ([DivisionId]) REFERENCES [dbo].[Divisions] ([Id]),
    CONSTRAINT [FK_Incomes_IncomeTypes] FOREIGN KEY ([IncomeTypeId]) REFERENCES [dbo].[IncomeTypes] ([Id]),
    CONSTRAINT [FK_Incomes_Users] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users] ([UserId])
);


GO
ALTER TABLE [dbo].[Incomes] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

