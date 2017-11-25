CREATE TABLE [dbo].[SpendingTypes] (
    [Id]         UNIQUEIDENTIFIER CONSTRAINT [DF_SpendingTypes_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [CompanyId]  UNIQUEIDENTIFIER NOT NULL,
    [DivisionId] UNIQUEIDENTIFIER NULL,
    [IsCommon]   BIT              CONSTRAINT [DF_SpendingTypes_IsCommon] DEFAULT ((0)) NOT NULL,
    [Name]       NVARCHAR (256)   NOT NULL,
    [IsDeleted]  BIT              CONSTRAINT [DF_SpendingTypes_IsActive] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_SpendingTypes] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_SpendingTypes_Companies] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Companies] ([CompanyId]),
    CONSTRAINT [FK_SpendingTypes_Divisions] FOREIGN KEY ([DivisionId]) REFERENCES [dbo].[Divisions] ([Id])
);


GO
ALTER TABLE [dbo].[SpendingTypes] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

