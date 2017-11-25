CREATE TABLE [dbo].[UnitCharges] (
    [Id]          UNIQUEIDENTIFIER CONSTRAINT [DF_UnitCharges_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [CompanyId]   UNIQUEIDENTIFIER NOT NULL,
    [TicketId]    UNIQUEIDENTIFIER NOT NULL,
    [UnitCharge]  INT              NOT NULL,
    [GuestCharge] INT              NOT NULL,
    [Reason]      NVARCHAR (250)   NULL,
    [Date]        DATETIME         NOT NULL,
    [CreatedBy]   UNIQUEIDENTIFIER NOT NULL,
    [EventId]     UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_UnitCharges] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UnitCharges_Tickets] FOREIGN KEY ([TicketId]) REFERENCES [dbo].[Tickets] ([Id]),
    CONSTRAINT [FK_UnitCharges_Users] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users] ([UserId])
);


GO
ALTER TABLE [dbo].[UnitCharges] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);


GO
CREATE NONCLUSTERED INDEX [IX_UnitCharges]
    ON [dbo].[UnitCharges]([EventId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_UC_Ticket]
    ON [dbo].[UnitCharges]([TicketId] ASC)
    INCLUDE([UnitCharge], [GuestCharge], [Id], [CompanyId], [Reason], [Date], [CreatedBy], [EventId]);


GO
CREATE NONCLUSTERED INDEX [IX_OPT_UnitCharges]
    ON [dbo].[UnitCharges]([CreatedBy] ASC)
    INCLUDE([Id], [CompanyId], [TicketId], [UnitCharge], [GuestCharge], [Reason], [Date], [EventId]);


GO
CREATE NONCLUSTERED INDEX [IX_UnitCharges_GetAvgSpendings]
    ON [dbo].[UnitCharges]([Date] ASC)
    INCLUDE([TicketId], [UnitCharge]);


GO
CREATE NONCLUSTERED INDEX [IX_CompanyID]
    ON [dbo].[UnitCharges]([CompanyId] ASC);

