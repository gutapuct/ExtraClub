CREATE TABLE [dbo].[VacationLists] (
    [Id]         UNIQUEIDENTIFIER CONSTRAINT [DF_VacationLists_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [CompanyId]  UNIQUEIDENTIFIER NOT NULL,
    [DivisionId] UNIQUEIDENTIFIER NOT NULL,
    [CreatedBy]  UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn]  DATETIME         NOT NULL,
    [Year]       INT              NOT NULL,
    CONSTRAINT [PK_VacationLists] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_VacationLists_Divisions] FOREIGN KEY ([DivisionId]) REFERENCES [dbo].[Divisions] ([Id]),
    CONSTRAINT [FK_VacationLists_Users] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users] ([UserId])
);


GO
ALTER TABLE [dbo].[VacationLists] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

