CREATE TABLE [dbo].[ReportParameters] (
    [Id]           UNIQUEIDENTIFIER CONSTRAINT [DF_ReportParameters_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [ReportInfoId] UNIQUEIDENTIFIER NOT NULL,
    [Name]         NVARCHAR (512)   NOT NULL,
    [IntName]      NVARCHAR (512)   NOT NULL,
    [Type]         INT              NOT NULL,
    [Order]        INT              CONSTRAINT [DF_ReportParameters_Order] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_ReportParameters] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ReportParameters_ReportInfos] FOREIGN KEY ([ReportInfoId]) REFERENCES [dbo].[ReportInfos] ([Id])
);


GO
ALTER TABLE [dbo].[ReportParameters] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

