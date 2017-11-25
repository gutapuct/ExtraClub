CREATE TABLE [dbo].[GoodActions] (
    [Id]        UNIQUEIDENTIFIER CONSTRAINT [DF_GoodActions_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [CompanyId] UNIQUEIDENTIFIER NOT NULL,
    [Name]      NVARCHAR (256)   NOT NULL,
    [IsActive]  BIT              NOT NULL,
    [Discount]  MONEY            NOT NULL,
    [CreatedOn] DATETIME         NOT NULL,
    [CreatedBy] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_GoodActions] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_GoodActions_Companies] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Companies] ([CompanyId]),
    CONSTRAINT [FK_GoodActions_Users] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users] ([UserId])
);


GO
ALTER TABLE [dbo].[GoodActions] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

