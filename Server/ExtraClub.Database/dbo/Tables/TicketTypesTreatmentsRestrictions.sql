CREATE TABLE [dbo].[TicketTypesTreatmentsRestrictions] (
    [TicketTypeId]    UNIQUEIDENTIFIER NOT NULL,
    [TreatmentTypeId] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_TicketTypesTreatmentsRestrictions] PRIMARY KEY CLUSTERED ([TicketTypeId] ASC, [TreatmentTypeId] ASC),
    CONSTRAINT [FK_TicketTypesTreatmentsRestrictions_TicketTypes] FOREIGN KEY ([TicketTypeId]) REFERENCES [dbo].[TicketTypes] ([Id]),
    CONSTRAINT [FK_TicketTypesTreatmentsRestrictions_TreatmentTypes] FOREIGN KEY ([TreatmentTypeId]) REFERENCES [dbo].[TreatmentTypes] ([Id])
);


GO
ALTER TABLE [dbo].[TicketTypesTreatmentsRestrictions] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

