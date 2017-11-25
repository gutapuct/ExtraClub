CREATE TABLE [dbo].[CustomerNotifications] (
    [Id]                UNIQUEIDENTIFIER CONSTRAINT [DF_CustomerNotifications_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [CustomerId]        UNIQUEIDENTIFIER NOT NULL,
    [CompanyId]         UNIQUEIDENTIFIER NOT NULL,
    [CreatedBy]         UNIQUEIDENTIFIER NULL,
    [Subject]           NVARCHAR (MAX)   NULL,
    [Message]           NVARCHAR (MAX)   NULL,
    [CompletedBy]       UNIQUEIDENTIFIER NULL,
    [CompletionComment] NVARCHAR (MAX)   NULL,
    [CreatedOn]         DATETIME         NOT NULL,
    [CompletedOn]       DATETIME         NULL,
    [ExpiryDate]        DATETIME         CONSTRAINT [DF_CustomerNotifications_ExpiryDate] DEFAULT (getdate()) NOT NULL,
    [Priority]          INT              CONSTRAINT [DF_CustomerNotifications_Priority] DEFAULT ((2)) NOT NULL,
    CONSTRAINT [PK_CustomerNotifications] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_CustomerNotifications_Companies] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Companies] ([CompanyId]),
    CONSTRAINT [FK_CustomerNotifications_Customers] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customers] ([Id]),
    CONSTRAINT [FK_CustomerNotifications_Users] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users] ([UserId]),
    CONSTRAINT [FK_CustomerNotifications_Users1] FOREIGN KEY ([CompletedBy]) REFERENCES [dbo].[Users] ([UserId])
);


GO
ALTER TABLE [dbo].[CustomerNotifications] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);


GO
CREATE NONCLUSTERED INDEX [IX_CUSTNOTIFICATIONS]
    ON [dbo].[CustomerNotifications]([CompanyId] ASC, [CompletedBy] ASC, [CreatedOn] ASC, [ExpiryDate] ASC)
    INCLUDE([Id], [CustomerId], [CreatedBy], [Subject], [Message], [CompletionComment], [CompletedOn], [Priority]);


GO
CREATE NONCLUSTERED INDEX [IX_CompanyId]
    ON [dbo].[CustomerNotifications]([CompanyId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CustomerID]
    ON [dbo].[CustomerNotifications]([CustomerId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CompletedBy]
    ON [dbo].[CustomerNotifications]([CompletedBy] ASC)
    INCLUDE([Id], [CustomerId], [CompanyId], [CreatedBy], [Subject], [Message], [CompletionComment], [CreatedOn], [CompletedOn], [ExpiryDate], [Priority]);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedBy]
    ON [dbo].[CustomerNotifications]([CreatedBy] ASC);

