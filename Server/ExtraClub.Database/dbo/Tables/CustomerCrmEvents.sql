CREATE TABLE [dbo].[CustomerCrmEvents] (
    [Id]         UNIQUEIDENTIFIER CONSTRAINT [DF_CustomerCrmEvents_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [CompanyId]  UNIQUEIDENTIFIER NOT NULL,
    [CustomerId] UNIQUEIDENTIFIER NOT NULL,
    [CreatedBy]  UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn]  DATETIME         NOT NULL,
    [EventDate]  DATETIME         NOT NULL,
    [Subject]    NVARCHAR (4000)  NOT NULL,
    [Comment]    NVARCHAR (MAX)   NULL,
    [DivisionId] UNIQUEIDENTIFIER NOT NULL,
    [Result]     NVARCHAR (4000)  NULL,
    CONSTRAINT [PK_CustomerCrmEvents] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_CustomerCrmEvents_Customers] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customers] ([Id]),
    CONSTRAINT [FK_CustomerCrmEvents_Users] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users] ([UserId])
);


GO
ALTER TABLE [dbo].[CustomerCrmEvents] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

