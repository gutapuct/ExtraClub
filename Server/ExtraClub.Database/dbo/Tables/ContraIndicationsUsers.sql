CREATE TABLE [dbo].[ContraIndicationsUsers] (
    [ContraIndicationId] UNIQUEIDENTIFIER NOT NULL,
    [CustomerId]         UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_ContraIndicationsUsers] PRIMARY KEY CLUSTERED ([ContraIndicationId] ASC, [CustomerId] ASC),
    CONSTRAINT [FK_ContraIndicationsUsers_ContraIndications] FOREIGN KEY ([ContraIndicationId]) REFERENCES [dbo].[ContraIndications] ([Id]),
    CONSTRAINT [FK_ContraIndicationsUsers_Customers] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customers] ([Id])
);


GO
ALTER TABLE [dbo].[ContraIndicationsUsers] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

