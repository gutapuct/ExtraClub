CREATE TABLE [dbo].[TicketPayments] (
    [Id]              UNIQUEIDENTIFIER CONSTRAINT [DF_TicketPayments_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [TicketId]        UNIQUEIDENTIFIER NOT NULL,
    [PaymentDate]     DATETIME         NOT NULL,
    [Amount]          MONEY            NOT NULL,
    [CreatedBy]       UNIQUEIDENTIFIER NOT NULL,
    [CompanyId]       UNIQUEIDENTIFIER NOT NULL,
    [ReceiptNumber]   INT              NULL,
    [RequestedAmount] MONEY            CONSTRAINT [DF_TicketPayments_RequestedAmount] DEFAULT ((0)) NOT NULL,
    [IsRoboCompleted] BIT              CONSTRAINT [DF_TicketPayments_IsRoboCompleted] DEFAULT ((0)) NOT NULL,
    [TRoboNumber]     INT              NULL,
    [BarOrderId]      UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_TicketPayments] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_TicketPayments_Tickets] FOREIGN KEY ([TicketId]) REFERENCES [dbo].[Tickets] ([Id]),
    CONSTRAINT [FK_TicketPayments_Users] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users] ([UserId])
);


GO
ALTER TABLE [dbo].[TicketPayments] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);


GO
CREATE NONCLUSTERED INDEX [IX_TicketPayments_TicketId]
    ON [dbo].[TicketPayments]([TicketId] ASC)
    INCLUDE([Id], [PaymentDate], [Amount], [CreatedBy], [CompanyId], [ReceiptNumber], [RequestedAmount], [IsRoboCompleted], [TRoboNumber], [BarOrderId]);


GO
CREATE NONCLUSTERED INDEX [IX_CRM_REPORT_4]
    ON [dbo].[TicketPayments]([BarOrderId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CompanyId]
    ON [dbo].[TicketPayments]([CompanyId] ASC);

