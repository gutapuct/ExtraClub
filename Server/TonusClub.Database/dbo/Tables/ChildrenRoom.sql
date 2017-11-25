CREATE TABLE [dbo].[ChildrenRoom] (
    [Id]           UNIQUEIDENTIFIER CONSTRAINT [DF_ChildrenRoom_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [CustomerId]   UNIQUEIDENTIFIER NOT NULL,
    [CompanyId]    UNIQUEIDENTIFIER NOT NULL,
    [DivisionId]   UNIQUEIDENTIFIER NOT NULL,
    [ChildName]    NVARCHAR (MAX)   NOT NULL,
    [CreatedBy]    UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn]    DATETIME         NOT NULL,
    [HealthStatus] NVARCHAR (MAX)   NULL,
    [Cost]         MONEY            NOT NULL,
    [OutTime]      DATETIME         NULL,
    [OutBy]        UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_ChildrenRoom] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ChildrenRoom_Companies] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Companies] ([CompanyId]),
    CONSTRAINT [FK_ChildrenRoom_Customers] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customers] ([Id]),
    CONSTRAINT [FK_ChildrenRoom_Divisions] FOREIGN KEY ([DivisionId]) REFERENCES [dbo].[Divisions] ([Id]),
    CONSTRAINT [FK_ChildrenRoom_Users] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users] ([UserId]),
    CONSTRAINT [FK_ChildrenRoom_Users1] FOREIGN KEY ([OutBy]) REFERENCES [dbo].[Users] ([UserId])
);


GO
ALTER TABLE [dbo].[ChildrenRoom] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

