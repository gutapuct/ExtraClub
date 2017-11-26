CREATE TABLE [dbo].[TreatmentsParalleling] (
    [TreatmentType1Id] UNIQUEIDENTIFIER NOT NULL,
    [TreatmentType2Id] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_TreatmentsParalleling] PRIMARY KEY CLUSTERED ([TreatmentType1Id] ASC, [TreatmentType2Id] ASC),
    CONSTRAINT [FK_TreatmentsParalleling_TreatmentTypes] FOREIGN KEY ([TreatmentType1Id]) REFERENCES [dbo].[TreatmentTypes] ([Id]),
    CONSTRAINT [FK_TreatmentsParalleling_TreatmentTypes1] FOREIGN KEY ([TreatmentType2Id]) REFERENCES [dbo].[TreatmentTypes] ([Id])
);


GO
ALTER TABLE [dbo].[TreatmentsParalleling] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

