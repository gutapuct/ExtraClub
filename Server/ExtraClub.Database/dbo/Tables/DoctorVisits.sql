CREATE TABLE [dbo].[DoctorVisits] (
    [Id]            UNIQUEIDENTIFIER CONSTRAINT [DF_DoctorVisits_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [CustomerId]    UNIQUEIDENTIFIER NOT NULL,
    [CompanyId]     UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn]     DATETIME         NOT NULL,
    [CreatedBy]     UNIQUEIDENTIFIER NOT NULL,
    [Doctor]        NVARCHAR (MAX)   NULL,
    [Date]          DATETIME         NULL,
    [Name]          NVARCHAR (MAX)   NULL,
    [Result]        NVARCHAR (MAX)   NULL,
    [DoctorComment] NVARCHAR (MAX)   NULL,
    CONSTRAINT [PK_DoctorVisits] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_DoctorVisits_Companies] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Companies] ([CompanyId]),
    CONSTRAINT [FK_DoctorVisits_Customers] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customers] ([Id]),
    CONSTRAINT [FK_DoctorVisits_Users] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users] ([UserId])
);


GO
ALTER TABLE [dbo].[DoctorVisits] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

