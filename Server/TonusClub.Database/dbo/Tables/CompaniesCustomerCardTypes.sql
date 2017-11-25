CREATE TABLE [dbo].[CompaniesCustomerCardTypes] (
    [CustomerCardTypeId] UNIQUEIDENTIFIER NOT NULL,
    [CompanyId]          UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_CompaniesCustomerCardTypes] PRIMARY KEY CLUSTERED ([CustomerCardTypeId] ASC, [CompanyId] ASC),
    CONSTRAINT [FK_CompaniesCustomerCardTypes_Companies] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Companies] ([CompanyId]),
    CONSTRAINT [FK_CompaniesCustomerCardTypes_CustomerCardTypes] FOREIGN KEY ([CustomerCardTypeId]) REFERENCES [dbo].[CustomerCardTypes] ([Id])
);


GO
ALTER TABLE [dbo].[CompaniesCustomerCardTypes] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

