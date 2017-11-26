CREATE TABLE [dbo].[AnketAdverts] (
    [Id]              UNIQUEIDENTIFIER CONSTRAINT [DF_AnketAdverts_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [CompanyId]       UNIQUEIDENTIFIER NOT NULL,
    [AnketId]         UNIQUEIDENTIFIER NOT NULL,
    [AdvertGroupName] NVARCHAR (512)   NOT NULL,
    [Name]            NVARCHAR (MAX)   NOT NULL,
    [Calls]           INT              NOT NULL,
    [Visits]          INT              NOT NULL,
    [Purchases]       INT              NOT NULL,
    [HadPlace]        BIT              NOT NULL,
    [HasComment]      BIT              NOT NULL,
    CONSTRAINT [PK_AnketAdverts] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AnketAdverts_Ankets] FOREIGN KEY ([AnketId]) REFERENCES [dbo].[Ankets] ([Id])
);


GO
ALTER TABLE [dbo].[AnketAdverts] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);


GO
CREATE NONCLUSTERED INDEX [IX_CompanyId]
    ON [dbo].[AnketAdverts]([CompanyId] ASC);

