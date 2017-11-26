CREATE TABLE [dbo].[TreatmentSeqRests] (
    [Id]               UNIQUEIDENTIFIER CONSTRAINT [DF_TreatmentSeqRests_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [TreatmentType1Id] UNIQUEIDENTIFIER NOT NULL,
    [TreatmentType2Id] UNIQUEIDENTIFIER NULL,
    [Interval]         INT              NULL,
    [Amount]           INT              NULL,
    CONSTRAINT [PK_TreatmentSeqRests] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_TreatmentSeqRests_TreatmentTypes] FOREIGN KEY ([TreatmentType1Id]) REFERENCES [dbo].[TreatmentTypes] ([Id]),
    CONSTRAINT [FK_TreatmentSeqRests_TreatmentTypes1] FOREIGN KEY ([TreatmentType2Id]) REFERENCES [dbo].[TreatmentTypes] ([Id])
);


GO
ALTER TABLE [dbo].[TreatmentSeqRests] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

