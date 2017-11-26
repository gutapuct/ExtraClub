CREATE TABLE [dbo].[ContraIndicationsTreatmentTypes] (
    [ContraIndicationId] UNIQUEIDENTIFIER NOT NULL,
    [TreatmentTypeId]    UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_ContraIndicationsTreatmentTypes] PRIMARY KEY CLUSTERED ([ContraIndicationId] ASC, [TreatmentTypeId] ASC),
    CONSTRAINT [FK_ContraIndicationsTreatmentTypes_ContraIndications] FOREIGN KEY ([ContraIndicationId]) REFERENCES [dbo].[ContraIndications] ([Id]),
    CONSTRAINT [FK_ContraIndicationsTreatmentTypes_TicketTypes] FOREIGN KEY ([TreatmentTypeId]) REFERENCES [dbo].[TreatmentTypes] ([Id])
);


GO
ALTER TABLE [dbo].[ContraIndicationsTreatmentTypes] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

