CREATE TABLE [dbo].[MinutesCharges] (
    [Id]            UNIQUEIDENTIFIER CONSTRAINT [DF_MinutesCharges_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [CompanyId]     UNIQUEIDENTIFIER NOT NULL,
    [TicketId]      UNIQUEIDENTIFIER NOT NULL,
    [MinutesCharge] MONEY            NOT NULL,
    [Reason]        NVARCHAR (250)   NULL,
    [Date]          DATETIME         NOT NULL,
    [CreatedBy]     UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_MinutesCharges] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_MinutesCharges_Tickets] FOREIGN KEY ([TicketId]) REFERENCES [dbo].[Tickets] ([Id]),
    CONSTRAINT [FK_MinutesCharges_Users] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users] ([UserId])
);


GO
ALTER TABLE [dbo].[MinutesCharges] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);


GO
CREATE NONCLUSTERED INDEX [IX_MinutesCharges_TicketId]
    ON [dbo].[MinutesCharges]([TicketId] ASC)
    INCLUDE([Id], [CompanyId], [MinutesCharge], [Reason], [Date], [CreatedBy]);


GO
CREATE NONCLUSTERED INDEX [IX_CompanyId]
    ON [dbo].[MinutesCharges]([CompanyId] ASC);

