CREATE TABLE [dbo].[Nutritions] (
    [Id]         UNIQUEIDENTIFIER CONSTRAINT [DF_Nutritions_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [CustomerId] UNIQUEIDENTIFIER NOT NULL,
    [CompanyId]  UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn]  DATETIME         NOT NULL,
    [CreatedBy]  UNIQUEIDENTIFIER NOT NULL,
    [Date]       DATETIME         NOT NULL,
    [Diet]       NVARCHAR (MAX)   NULL,
    [Product]    NVARCHAR (MAX)   NULL,
    [Weight]     NVARCHAR (MAX)   NULL,
    [Proteins]   NVARCHAR (MAX)   NULL,
    [Fats]       NVARCHAR (MAX)   NULL,
    [Carbos]     NVARCHAR (MAX)   NULL,
    [Ccals]      NVARCHAR (MAX)   NULL,
    [Comments]   NVARCHAR (MAX)   NULL,
    CONSTRAINT [PK_Nutritions] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Nutritions_Companies] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Companies] ([CompanyId]),
    CONSTRAINT [FK_Nutritions_Customers] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customers] ([Id]),
    CONSTRAINT [FK_Nutritions_Users] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users] ([UserId])
);


GO
ALTER TABLE [dbo].[Nutritions] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

