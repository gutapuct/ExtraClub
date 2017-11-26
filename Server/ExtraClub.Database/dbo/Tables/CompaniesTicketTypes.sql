CREATE TABLE [dbo].[CompaniesTicketTypes] (
    [TicketTypeId] UNIQUEIDENTIFIER NOT NULL,
    [CompanyId]    UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_CompaniesTicketTypes] PRIMARY KEY CLUSTERED ([TicketTypeId] ASC, [CompanyId] ASC),
    CONSTRAINT [FK_CompaniesTicketTypes_Companies] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Companies] ([CompanyId]),
    CONSTRAINT [FK_CompaniesTicketTypes_TicketTypes] FOREIGN KEY ([TicketTypeId]) REFERENCES [dbo].[TicketTypes] ([Id])
);


GO
ALTER TABLE [dbo].[CompaniesTicketTypes] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

