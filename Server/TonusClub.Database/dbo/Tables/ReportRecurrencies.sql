CREATE TABLE [dbo].[ReportRecurrencies] (
    [Id]           UNIQUEIDENTIFIER CONSTRAINT [DF_ReportRecurrencies_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [CompanyId]    UNIQUEIDENTIFIER NOT NULL,
    [UserId]       UNIQUEIDENTIFIER NOT NULL,
    [ReportKey]    NVARCHAR (100)   NOT NULL,
    [Recurrency]   INT              NOT NULL,
    [PeriodDay]    INT              NOT NULL,
    [Parameters]   NVARCHAR (MAX)   NOT NULL,
    [LastSentDate] DATETIME         NULL,
    CONSTRAINT [PK_ReportRecurrencies] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
ALTER TABLE [dbo].[ReportRecurrencies] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

