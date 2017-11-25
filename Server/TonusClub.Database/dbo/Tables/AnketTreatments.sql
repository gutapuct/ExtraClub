CREATE TABLE [dbo].[AnketTreatments] (
    [Id]              UNIQUEIDENTIFIER CONSTRAINT [DF_AnketTreatments_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [CompanyId]       UNIQUEIDENTIFIER NOT NULL,
    [AnketId]         UNIQUEIDENTIFIER NOT NULL,
    [TreatmentTypeId] UNIQUEIDENTIFIER NOT NULL,
    [Amount]          INT              NOT NULL,
    CONSTRAINT [PK_AnketTreatments] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AnketTreatments_Ankets] FOREIGN KEY ([AnketId]) REFERENCES [dbo].[Ankets] ([Id]),
    CONSTRAINT [FK_AnketTreatments_TreatmentTypes] FOREIGN KEY ([TreatmentTypeId]) REFERENCES [dbo].[TreatmentTypes] ([Id])
);


GO
ALTER TABLE [dbo].[AnketTreatments] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);


GO
CREATE NONCLUSTERED INDEX [IX_CompayId]
    ON [dbo].[AnketTreatments]([CompanyId] ASC);

