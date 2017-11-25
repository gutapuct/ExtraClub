CREATE TABLE [dbo].[CustomerTargets] (
    [Id]                     UNIQUEIDENTIFIER CONSTRAINT [DF_CustomerTargets_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [CustomerId]             UNIQUEIDENTIFIER NOT NULL,
    [CompanyId]              UNIQUEIDENTIFIER NOT NULL,
    [TargetText]             NVARCHAR (MAX)   NOT NULL,
    [TargetDate]             DATETIME         NOT NULL,
    [Method]                 NVARCHAR (MAX)   NULL,
    [RecomendationsFollowed] BIT              NULL,
    [TargetComplete]         BIT              NULL,
    [Comment]                NVARCHAR (MAX)   NULL,
    [CreatedBy]              UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn]              DATETIME         NOT NULL,
    [TargetTypeId]           UNIQUEIDENTIFIER CONSTRAINT [DF_CustomerTargets_TargetTypeId] DEFAULT ('00000000-0000-0000-0000-000000000000') NOT NULL,
    CONSTRAINT [PK_CustomerTargets] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_CustomerTargets_Companies] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Companies] ([CompanyId]),
    CONSTRAINT [FK_CustomerTargets_Customers] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customers] ([Id]),
    CONSTRAINT [FK_CustomerTargets_CustomerTargetTypes] FOREIGN KEY ([TargetTypeId]) REFERENCES [dbo].[CustomerTargetTypes] ([Id]),
    CONSTRAINT [FK_CustomerTargets_Users] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users] ([UserId])
);


GO
ALTER TABLE [dbo].[CustomerTargets] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);


GO
CREATE NONCLUSTERED INDEX [IX_CompanyId]
    ON [dbo].[CustomerTargets]([CompanyId] ASC);

