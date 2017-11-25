CREATE TABLE [dbo].[Calls] (
    [Id]         UNIQUEIDENTIFIER CONSTRAINT [DF_Calls_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [CompanyId]  UNIQUEIDENTIFIER NOT NULL,
    [DivisionId] UNIQUEIDENTIFIER NOT NULL,
    [CreatedBy]  UNIQUEIDENTIFIER NOT NULL,
    [StartAt]    DATETIME         NOT NULL,
    [Log]        NVARCHAR (MAX)   NOT NULL,
    [ResultType] INT              NOT NULL,
    [IsIncoming] BIT              NOT NULL,
    [CustomerId] UNIQUEIDENTIFIER NULL,
    [Goal]       NVARCHAR (4000)  NULL,
    [Result]     NVARCHAR (4000)  NULL,
    CONSTRAINT [PK_Calls] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Calls_Customers] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customers] ([Id]),
    CONSTRAINT [FK_Calls_Divisions] FOREIGN KEY ([DivisionId]) REFERENCES [dbo].[Divisions] ([Id]),
    CONSTRAINT [FK_Calls_Users] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users] ([UserId])
);


GO
ALTER TABLE [dbo].[Calls] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedBy]
    ON [dbo].[Calls]([CreatedBy] ASC)
    INCLUDE([Id], [CompanyId], [DivisionId], [StartAt], [Log], [ResultType], [IsIncoming], [CustomerId], [Goal], [Result]);


GO
CREATE NONCLUSTERED INDEX IX_CALLS_OPT_1
	ON [dbo].[Calls] ([DivisionId],[StartAt])
	INCLUDE ([Id],[CompanyId],[CreatedBy],[Log],[ResultType],[IsIncoming],[CustomerId],[Goal],[Result])
