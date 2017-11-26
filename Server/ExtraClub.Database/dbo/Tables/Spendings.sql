CREATE TABLE [dbo].[Spendings] (
    [Id]             UNIQUEIDENTIFIER CONSTRAINT [DF_Table_1_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [CompanyId]      UNIQUEIDENTIFIER NOT NULL,
    [DivisionId]     UNIQUEIDENTIFIER NULL,
    [CreatedOn]      DATETIME         NOT NULL,
    [CreatedBy]      UNIQUEIDENTIFIER NOT NULL,
    [Number]         INT              NOT NULL,
    [Name]           NVARCHAR (MAX)   NULL,
    [SpendingTypeId] UNIQUEIDENTIFIER NOT NULL,
    [Amount]         MONEY            NOT NULL,
    [PaymentType]    NVARCHAR (512)   NULL,
    [IsInvestment]   BIT              CONSTRAINT [DF_Spendings_IsInvestment] DEFAULT ((0)) NOT NULL,
    [IsFinAction]    BIT              CONSTRAINT [DF_Spendings_IsFinAction] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Spendings] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Table_1_Companies] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Companies] ([CompanyId]),
    CONSTRAINT [FK_Table_1_Divisions] FOREIGN KEY ([DivisionId]) REFERENCES [dbo].[Divisions] ([Id]),
    CONSTRAINT [FK_Table_1_SpendingTypes] FOREIGN KEY ([SpendingTypeId]) REFERENCES [dbo].[SpendingTypes] ([Id]),
    CONSTRAINT [FK_Table_1_Users] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users] ([UserId])
);


GO
ALTER TABLE [dbo].[Spendings] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);


GO
CREATE NONCLUSTERED INDEX [IX_CompanyId]
    ON [dbo].[Spendings]([CompanyId] ASC);

