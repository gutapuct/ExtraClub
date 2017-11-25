CREATE TABLE [dbo].[SalesPlans] (
    [Id]         UNIQUEIDENTIFIER CONSTRAINT [DF_SalesPlans_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [CompanyId]  UNIQUEIDENTIFIER NOT NULL,
    [DivisionId] UNIQUEIDENTIFIER NOT NULL,
    [Month]      DATE             NOT NULL,
    [Value]      MONEY            NOT NULL,
    [CorpValue]  MONEY            CONSTRAINT [DF_SalesPlans_CorpValue] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_SalesPlans] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_SalesPlans_Divisions] FOREIGN KEY ([DivisionId]) REFERENCES [dbo].[Divisions] ([Id])
);


GO
ALTER TABLE [dbo].[SalesPlans] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

