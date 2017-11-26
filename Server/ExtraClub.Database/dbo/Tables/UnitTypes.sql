CREATE TABLE [dbo].[UnitTypes] (
    [Id]         UNIQUEIDENTIFIER NOT NULL,
    [Name]       NVARCHAR (MAX)   NOT NULL,
    [CreatedOn]  DATETIME         NOT NULL,
    [ModifiedOn] DATETIME         NULL,
    [AuthorId]   UNIQUEIDENTIFIER NOT NULL,
    [FirebirdId] INT              NULL,
    [IsAvail]    BIT              CONSTRAINT [DF_UnitTypes_IsAvail] DEFAULT ((1)) NOT NULL,
    [NameEn]     NVARCHAR (256)   NULL,
    CONSTRAINT [PK_UnitTypes] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
ALTER TABLE [dbo].[UnitTypes] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);


GO
CREATE NONCLUSTERED INDEX [IX_FK_UserUnitType]
    ON [dbo].[UnitTypes]([AuthorId] ASC);

