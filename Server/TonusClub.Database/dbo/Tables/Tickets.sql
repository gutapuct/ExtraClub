CREATE TABLE [dbo].[Tickets] (
    [Id]                    UNIQUEIDENTIFIER CONSTRAINT [DF_Tickets_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [CustomerId]            UNIQUEIDENTIFIER NOT NULL,
    [DivisionId]            UNIQUEIDENTIFIER NOT NULL,
    [TicketTypeId]          UNIQUEIDENTIFIER NOT NULL,
    [CreatedBy]             UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn]             DATETIME         CONSTRAINT [DF_Tickets_CreatedOn] DEFAULT (getdate()) NOT NULL,
    [StartDate]             DATE             NULL,
    [Price]                 MONEY            NOT NULL,
    [DiscountPercent]       DECIMAL (6, 4)   CONSTRAINT [DF_Tickets_DiscountPercent] DEFAULT ((0)) NOT NULL,
    [UnitsAmount]           MONEY            CONSTRAINT [DF_Tickets_UnitsAmount] DEFAULT ((0)) NOT NULL,
    [GuestUnitsAmount]      MONEY            CONSTRAINT [DF_Tickets_GuestUnitsAmount] DEFAULT ((0)) NOT NULL,
    [Number]                NVARCHAR (20)    NOT NULL,
    [Length]                INT              NOT NULL,
    [ReturnCost]            MONEY            NULL,
    [ReturnDate]            DATE             NULL,
    [ReturnUserId]          UNIQUEIDENTIFIER NULL,
    [InheritedTicketId]     UNIQUEIDENTIFIER NULL,
    [IsActive]              BIT              CONSTRAINT [DF_Tickets_IsActive] DEFAULT ((0)) NOT NULL,
    [CompanyId]             UNIQUEIDENTIFIER NOT NULL,
    [InstalmentId]          UNIQUEIDENTIFIER NULL,
    [LastInstalmentDay]     DATE             NULL,
    [FreezesAmount]         INT              CONSTRAINT [DF_Tickets_FreezesAmount] DEFAULT ((0)) NOT NULL,
    [SolariumMinutes]       MONEY            CONSTRAINT [DF_Tickets_SolariumMinutes_1] DEFAULT ((0)) NOT NULL,
    [HasNotify]             BIT              CONSTRAINT [DF_Tickets_HasNotify] DEFAULT ((0)) NOT NULL,
    [FirstPmtTypeId]        INT              CONSTRAINT [DF_Tickets_FirstPmtTypeId] DEFAULT ((0)) NOT NULL,
    [ModifiedBy]            UNIQUEIDENTIFIER NULL,
    [ModifiedOn]            DATETIME         NULL,
    [InvoiceNumber]         INT              NULL,
    [VatAmount]             MONEY            NULL,
    [PlanningInstalmentDay] DATE             NULL,
    [CreditInitialPayment]  MONEY            NULL,
    [CreditComment]         NVARCHAR (MAX)   NULL,
    [CreditComission]       MONEY            NULL,
    [Comment]               NVARCHAR (4000)  NULL,
    CONSTRAINT [PK_Tickets] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Tickets_Customers] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customers] ([Id]),
    CONSTRAINT [FK_Tickets_Divisions] FOREIGN KEY ([DivisionId]) REFERENCES [dbo].[Divisions] ([Id]),
    CONSTRAINT [FK_Tickets_Instalments] FOREIGN KEY ([InstalmentId]) REFERENCES [dbo].[Instalments] ([Id]),
    CONSTRAINT [FK_Tickets_Tickets] FOREIGN KEY ([InheritedTicketId]) REFERENCES [dbo].[Tickets] ([Id]),
    CONSTRAINT [FK_Tickets_TicketTypes] FOREIGN KEY ([TicketTypeId]) REFERENCES [dbo].[TicketTypes] ([Id]),
    CONSTRAINT [FK_Tickets_Users] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users] ([UserId]),
    CONSTRAINT [FK_Tickets_Users1] FOREIGN KEY ([ReturnUserId]) REFERENCES [dbo].[Users] ([UserId])
);


GO
ALTER TABLE [dbo].[Tickets] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);


GO
CREATE NONCLUSTERED INDEX [IX_TicketsByCustomer]
    ON [dbo].[Tickets]([CustomerId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Successors_Large]
    ON [dbo].[Tickets]([InheritedTicketId] ASC)
    INCLUDE([Id], [CustomerId], [DivisionId], [TicketTypeId], [CreatedBy], [CreatedOn], [StartDate], [Price], [DiscountPercent], [UnitsAmount], [GuestUnitsAmount], [Number], [Length], [ReturnCost], [ReturnDate], [ReturnUserId], [IsActive], [CompanyId], [InstalmentId], [LastInstalmentDay], [FreezesAmount], [SolariumMinutes], [HasNotify], [FirstPmtTypeId], [ModifiedBy], [ModifiedOn], [InvoiceNumber], [VatAmount], [PlanningInstalmentDay], [CreditInitialPayment], [CreditComment], [CreditComission]);


GO
CREATE NONCLUSTERED INDEX [IX_TICKETS_MARKETING]
    ON [dbo].[Tickets]([DivisionId] ASC)
    INCLUDE([Id], [TicketTypeId], [StartDate], [Length]);


GO
CREATE NONCLUSTERED INDEX [IX_TICKETS_Rep1]
    ON [dbo].[Tickets]([UnitsAmount] ASC, [Length] ASC)
    INCLUDE([Id], [StartDate]);


GO
CREATE NONCLUSTERED INDEX [IX_TICKETS_Rep2]
    ON [dbo].[Tickets]([UnitsAmount] ASC, [Length] ASC)
    INCLUDE([Id], [StartDate], [Price], [DiscountPercent], [CompanyId]);


GO
CREATE NONCLUSTERED INDEX [IX_CRM_REPORT_1]
    ON [dbo].[Tickets]([CreatedOn] ASC)
    INCLUDE([DivisionId], [TicketTypeId]);


GO
CREATE NONCLUSTERED INDEX [IX_CompanyId]
    ON [dbo].[Tickets]([CompanyId] ASC)
    INCLUDE([ReturnDate], [Id], [CustomerId], [DivisionId], [TicketTypeId], [CreatedBy], [CreatedOn], [StartDate], [Price], [DiscountPercent], [UnitsAmount], [GuestUnitsAmount], [Number], [Length], [ReturnCost], [ReturnUserId], [InheritedTicketId], [IsActive], [InstalmentId], [LastInstalmentDay], [FreezesAmount], [SolariumMinutes], [HasNotify], [FirstPmtTypeId], [ModifiedBy], [ModifiedOn], [InvoiceNumber], [VatAmount], [PlanningInstalmentDay], [CreditInitialPayment], [CreditComment], [CreditComission], [Comment]);

