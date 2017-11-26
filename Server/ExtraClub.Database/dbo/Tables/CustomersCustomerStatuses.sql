CREATE TABLE [dbo].[CustomersCustomerStatuses] (
    [CustomerStatusId] UNIQUEIDENTIFIER NOT NULL,
    [CustomerId]       UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_CustomersCustomerStatuses] PRIMARY KEY CLUSTERED ([CustomerStatusId] ASC, [CustomerId] ASC),
    CONSTRAINT [FK_CustomersCustomerStatuses_Customers] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customers] ([Id]),
    CONSTRAINT [FK_CustomersCustomerStatuses_CustomerStatuses] FOREIGN KEY ([CustomerStatusId]) REFERENCES [dbo].[CustomerStatuses] ([Id])
);


GO
ALTER TABLE [dbo].[CustomersCustomerStatuses] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

