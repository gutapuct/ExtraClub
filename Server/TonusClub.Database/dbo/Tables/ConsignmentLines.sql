CREATE TABLE [dbo].[ConsignmentLines] (
    [Id]            UNIQUEIDENTIFIER CONSTRAINT [DF_ConsignmentLines_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [ConsignmentId] UNIQUEIDENTIFIER NOT NULL,
    [GoodId]        UNIQUEIDENTIFIER NOT NULL,
    [Quantity]      FLOAT (53)       NULL,
    [Price]         MONEY            NULL,
    [Expiry]        SMALLDATETIME    NULL,
    [CreatedBy]     UNIQUEIDENTIFIER NOT NULL,
    [CompanyId]     UNIQUEIDENTIFIER NOT NULL,
    [Position]      SMALLINT         NULL,
    [Comment]       NVARCHAR (512)   NULL,
    CONSTRAINT [PK_ConsignmentLines] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ConsignmentLines_Consignments] FOREIGN KEY ([ConsignmentId]) REFERENCES [dbo].[Consignments] ([Id]),
    CONSTRAINT [FK_ConsignmentLines_Goods] FOREIGN KEY ([GoodId]) REFERENCES [dbo].[Goods] ([GoodId]),
    CONSTRAINT [FK_ConsignmentLines_Users] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users] ([UserId])
);


GO
ALTER TABLE [dbo].[ConsignmentLines] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);


GO
CREATE NONCLUSTERED INDEX [IX_CompanyId]
    ON [dbo].[ConsignmentLines]([CompanyId] ASC);

