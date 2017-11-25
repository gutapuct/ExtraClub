CREATE TABLE [dbo].[GoodActionLines] (
    [Id]           UNIQUEIDENTIFIER CONSTRAINT [DF_GoodActionLines_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [GoodActionId] UNIQUEIDENTIFIER NOT NULL,
    [GoodId]       UNIQUEIDENTIFIER NOT NULL,
    [Amount]       INT              CONSTRAINT [DF_GoodActionLines_Amount] DEFAULT ((1)) NOT NULL,
    [CompanyId]    UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_GoodActionLines] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_GoodActionLines_GoodActions] FOREIGN KEY ([GoodActionId]) REFERENCES [dbo].[GoodActions] ([Id]),
    CONSTRAINT [FK_GoodActionLines_Goods] FOREIGN KEY ([GoodId]) REFERENCES [dbo].[Goods] ([GoodId])
);


GO
ALTER TABLE [dbo].[GoodActionLines] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

