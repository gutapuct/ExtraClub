CREATE TABLE [dbo].[CustomerTargetTypesTreatmentConfigs] (
    [CustomerTargetTypeId] UNIQUEIDENTIFIER NOT NULL,
    [TreatmentConfigId]    UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_CustomerTargetTypesTreatmentConfigs] PRIMARY KEY CLUSTERED ([CustomerTargetTypeId] ASC, [TreatmentConfigId] ASC),
    CONSTRAINT [FK_CustomerTargetTypesTreatmentConfigs_CustomerTargetTypes] FOREIGN KEY ([CustomerTargetTypeId]) REFERENCES [dbo].[CustomerTargetTypes] ([Id]),
    CONSTRAINT [FK_CustomerTargetTypesTreatmentConfigs_TreatmentConfigs] FOREIGN KEY ([TreatmentConfigId]) REFERENCES [dbo].[TreatmentConfig] ([Id])
);


GO
ALTER TABLE [dbo].[CustomerTargetTypesTreatmentConfigs] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

