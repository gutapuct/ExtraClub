CREATE TABLE [dbo].[CustomerNotificationsEmployees] (
    [CustomerNotificationId] UNIQUEIDENTIFIER NOT NULL,
    [EmployeeId]             UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_CustomerNotificationsEmployees] PRIMARY KEY CLUSTERED ([CustomerNotificationId] ASC, [EmployeeId] ASC),
    CONSTRAINT [FK_CustomerNotificationsEmployees_CustomerNotifications] FOREIGN KEY ([CustomerNotificationId]) REFERENCES [dbo].[CustomerNotifications] ([Id]),
    CONSTRAINT [FK_CustomerNotificationsEmployees_Employees] FOREIGN KEY ([EmployeeId]) REFERENCES [dbo].[Employees] ([Id])
);


GO
ALTER TABLE [dbo].[CustomerNotificationsEmployees] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

