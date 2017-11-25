CREATE TABLE [dbo].[VacationListItems] (
    [Id]             UNIQUEIDENTIFIER CONSTRAINT [DF_VacationListItems_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [CompanyId]      UNIQUEIDENTIFIER NOT NULL,
    [VacationListId] UNIQUEIDENTIFIER NOT NULL,
    [EmployeeId]     UNIQUEIDENTIFIER NOT NULL,
    [StartDate]      DATE             NOT NULL,
    [FinishDate]     DATE             NOT NULL,
    CONSTRAINT [PK_VacationListItems] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_VacationListItems_Employees] FOREIGN KEY ([EmployeeId]) REFERENCES [dbo].[Employees] ([Id]),
    CONSTRAINT [FK_VacationListItems_VacationLists] FOREIGN KEY ([VacationListId]) REFERENCES [dbo].[VacationLists] ([Id])
);


GO
ALTER TABLE [dbo].[VacationListItems] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

