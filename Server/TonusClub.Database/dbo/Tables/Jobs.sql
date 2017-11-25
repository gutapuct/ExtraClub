CREATE TABLE [dbo].[Jobs] (
    [Id]                UNIQUEIDENTIFIER CONSTRAINT [DF_Jobs_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [CompanyId]         UNIQUEIDENTIFIER NOT NULL,
    [DivisionId]        UNIQUEIDENTIFIER NOT NULL,
    [Name]              NVARCHAR (250)   NOT NULL,
    [Duties]            NVARCHAR (MAX)   NULL,
    [Unit]              NVARCHAR (250)   NULL,
    [Salary]            MONEY            NOT NULL,
    [Vacansies]         INT              NOT NULL,
    [ParallelVacansies] INT              NOT NULL,
    [IsMainWorkplace]   BIT              NOT NULL,
    [WorkGraph]         NVARCHAR (50)    NOT NULL,
    [WorkStart]         TIME (7)         NOT NULL,
    [WorkEnd]           TIME (7)         NOT NULL,
    [CreatedOn]         DATETIME         CONSTRAINT [DF_Jobs_CreatedOn_1] DEFAULT (getdate()) NOT NULL,
    [BeselinedBy]       UNIQUEIDENTIFIER NULL,
    [BaselinedOn]       DATETIME         NULL,
    [HiddenOn]          DATETIME         NULL,
    [SalarySchemeId]    UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_Jobs] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Jobs_Companies] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Companies] ([CompanyId]),
    CONSTRAINT [FK_Jobs_Divisions] FOREIGN KEY ([DivisionId]) REFERENCES [dbo].[Divisions] ([Id]),
    CONSTRAINT [FK_Jobs_SalaryScheme] FOREIGN KEY ([SalarySchemeId]) REFERENCES [dbo].[SalaryScheme] ([Id]),
    CONSTRAINT [FK_Jobs_Users] FOREIGN KEY ([BeselinedBy]) REFERENCES [dbo].[Users] ([UserId])
);


GO
ALTER TABLE [dbo].[Jobs] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

