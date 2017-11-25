CREATE TABLE [dbo].[Employees] (
    [Id]              UNIQUEIDENTIFIER CONSTRAINT [DF_Employees_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [CompanyId]       UNIQUEIDENTIFIER NOT NULL,
    [MainDivisionId]  UNIQUEIDENTIFIER NOT NULL,
    [CreatedBy]       UNIQUEIDENTIFIER NOT NULL,
    [BoundCustomerId] UNIQUEIDENTIFIER NOT NULL,
    [FactIndex]       NVARCHAR (50)    NULL,
    [FactCity]        NVARCHAR (250)   NULL,
    [FactStreet]      NVARCHAR (250)   NULL,
    [FactOther]       NVARCHAR (250)   NULL,
    [FactMetro]       NVARCHAR (50)    NULL,
    [CardBarcode]     INT              NULL,
    [Number]          INT              CONSTRAINT [DF_Employees_Number] DEFAULT ((0)) NOT NULL,
    [IsActive]        BIT              CONSTRAINT [DF_Employees_IsActive] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_Employees] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Employees_Companies] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Companies] ([CompanyId]),
    CONSTRAINT [FK_Employees_Customers] FOREIGN KEY ([BoundCustomerId]) REFERENCES [dbo].[Customers] ([Id]),
    CONSTRAINT [FK_Employees_Divisions] FOREIGN KEY ([MainDivisionId]) REFERENCES [dbo].[Divisions] ([Id]),
    CONSTRAINT [FK_Employees_Users] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users] ([UserId])
);


GO
ALTER TABLE [dbo].[Employees] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

