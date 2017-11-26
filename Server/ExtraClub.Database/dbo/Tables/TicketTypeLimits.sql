CREATE TABLE [dbo].[TicketTypeLimits] (
    [Id]                UNIQUEIDENTIFIER CONSTRAINT [DF_TicketTypeLimits_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [TicketTypeId]      UNIQUEIDENTIFIER NOT NULL,
    [TreatmentConfigId] UNIQUEIDENTIFIER NOT NULL,
    [Amount]            INT              NOT NULL,
    CONSTRAINT [PK_TicketTypeLimits] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_TicketTypeLimits_TicketTypes] FOREIGN KEY ([TicketTypeId]) REFERENCES [dbo].[TicketTypes] ([Id]),
    CONSTRAINT [FK_TicketTypeLimits_TreatmentConfig] FOREIGN KEY ([TreatmentConfigId]) REFERENCES [dbo].[TreatmentConfig] ([Id])
);


GO
ALTER TABLE [dbo].[TicketTypeLimits] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

