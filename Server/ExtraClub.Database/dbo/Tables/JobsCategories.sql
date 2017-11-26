CREATE TABLE [dbo].[JobsCategories] (
    [JobId]      UNIQUEIDENTIFIER NOT NULL,
    [CategoryId] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_JobsCategories] PRIMARY KEY CLUSTERED ([JobId] ASC, [CategoryId] ASC),
    CONSTRAINT [FK_JobsCategories_EmployeeCategories] FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[EmployeeCategories] ([Id]),
    CONSTRAINT [FK_JobsCategories_Jobs] FOREIGN KEY ([JobId]) REFERENCES [dbo].[Jobs] ([Id])
);


GO
ALTER TABLE [dbo].[JobsCategories] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

