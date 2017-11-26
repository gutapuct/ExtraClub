CREATE TABLE [dbo].[EmployeeWorkGraphs] (
    [Id]             UNIQUEIDENTIFIER CONSTRAINT [DF_EmployeeWorkGraphs_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [CompanyId]      UNIQUEIDENTIFIER NOT NULL,
    [DivisionId]     UNIQUEIDENTIFIER NOT NULL,
    [CreatedBy]      UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn]      DATETIME         NOT NULL,
    [Begin]          DATE             NOT NULL,
    [End]            DATE             NOT NULL,
    [SerializedData] VARBINARY (MAX)  NOT NULL,
    CONSTRAINT [PK_EmployeeWorkGraphs] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_EmployeeWorkGraphs_Divisions] FOREIGN KEY ([DivisionId]) REFERENCES [dbo].[Divisions] ([Id]),
    CONSTRAINT [FK_EmployeeWorkGraphs_Users] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users] ([UserId])
);


GO
ALTER TABLE [dbo].[EmployeeWorkGraphs] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

