CREATE TABLE [dbo].[CustomerMeasures] (
    [Id]         UNIQUEIDENTIFIER CONSTRAINT [DF_CustomerMeasures_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [CustomerId] UNIQUEIDENTIFIER NOT NULL,
    [CompanyId]  UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn]  DATETIME         NOT NULL,
    [CreatedBy]  UNIQUEIDENTIFIER NOT NULL,
    [Date]       DATETIME         NOT NULL,
    [LoadType]   NVARCHAR (MAX)   NULL,
    [AD0Up]      NVARCHAR (MAX)   NULL,
    [AD0Down]    NVARCHAR (MAX)   NULL,
    [PS0]        NVARCHAR (MAX)   NULL,
    [AD1Up]      NVARCHAR (MAX)   NULL,
    [AD1Down]    NVARCHAR (MAX)   NULL,
    [PS1]        NVARCHAR (MAX)   NULL,
    [AD2Up]      NVARCHAR (MAX)   NULL,
    [AD2Down]    NVARCHAR (MAX)   NULL,
    [PS2]        NVARCHAR (MAX)   NULL,
    [AD3Up]      NVARCHAR (MAX)   NULL,
    [AD3Down]    NVARCHAR (MAX)   NULL,
    [PS3]        NVARCHAR (MAX)   NULL,
    [Conclusion] NVARCHAR (MAX)   NULL,
    CONSTRAINT [PK_CustomerMeasures] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_CustomerMeasures_Companies] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Companies] ([CompanyId]),
    CONSTRAINT [FK_CustomerMeasures_Customers] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customers] ([Id]),
    CONSTRAINT [FK_CustomerMeasures_Users] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users] ([UserId])
);


GO
ALTER TABLE [dbo].[CustomerMeasures] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

