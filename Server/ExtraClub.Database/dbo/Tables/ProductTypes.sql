CREATE TABLE [dbo].[ProductTypes] (
    [Id]              UNIQUEIDENTIFIER NOT NULL,
    [ProductTypeName] NVARCHAR (MAX)   NOT NULL,
    [SalesPercent]    DECIMAL (18)     NOT NULL,
    [CreatedOn]       DATETIME         NOT NULL,
    [ModifiedOn]      DATETIME         NULL,
    [AuthorId]        UNIQUEIDENTIFIER NOT NULL,
    [FirebirdId]      INT              NULL,
    [IsAvail]         BIT              CONSTRAINT [DF_ProductTypes_IsAvail] DEFAULT ((1)) NOT NULL,
    [NameEn]          NVARCHAR (256)   NULL,
    CONSTRAINT [PK_ProductTypes] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
ALTER TABLE [dbo].[ProductTypes] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);


GO
CREATE NONCLUSTERED INDEX [IX_FK_ProductTypeUser]
    ON [dbo].[ProductTypes]([AuthorId] ASC);

