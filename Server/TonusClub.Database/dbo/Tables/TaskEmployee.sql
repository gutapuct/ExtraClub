CREATE TABLE [dbo].[TaskEmployee] (
    [TaskId]     UNIQUEIDENTIFIER NOT NULL,
    [EmployeeId] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_TaskEmployee] PRIMARY KEY CLUSTERED ([TaskId] ASC, [EmployeeId] ASC),
    CONSTRAINT [FK_TaskEmployee_Employees] FOREIGN KEY ([EmployeeId]) REFERENCES [dbo].[Employees] ([Id]),
    CONSTRAINT [FK_TaskEmployee_Tasks] FOREIGN KEY ([TaskId]) REFERENCES [dbo].[Tasks] ([Id])
);


GO
ALTER TABLE [dbo].[TaskEmployee] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

