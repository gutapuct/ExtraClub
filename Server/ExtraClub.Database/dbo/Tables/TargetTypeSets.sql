CREATE TABLE [dbo].[TargetTypeSets] (
    [Id]                 UNIQUEIDENTIFIER NOT NULL,
    [TargetTypeId]       UNIQUEIDENTIFIER NOT NULL,
    [TreatmentConfigIds] NVARCHAR (4000)  NOT NULL,
    CONSTRAINT [PK_TargetTypeSets] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_TargetTypeSets_CustomerTargetTypes] FOREIGN KEY ([TargetTypeId]) REFERENCES [dbo].[CustomerTargetTypes] ([Id])
);


GO
ALTER TABLE [dbo].[TargetTypeSets] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

