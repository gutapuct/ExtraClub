CREATE TABLE [dbo].[LocalSettings] (
    [Id]                  INT              NOT NULL,
    [LastSyncVersion]     BIGINT           NOT NULL,
    [DbVersion]           INT              NOT NULL,
    [NotifyLicenseDays]   INT              CONSTRAINT [DF_LocalSettings_NotifyLicenseDays] DEFAULT ((0)) NOT NULL,
    [NotifyLicensePeriod] INT              CONSTRAINT [DF_LocalSettings_NotifyLicensePeriod] DEFAULT ((0)) NOT NULL,
    [NotifyKeyDays]       INT              CONSTRAINT [DF_LocalSettings_NotifyKeyDays] DEFAULT ((0)) NOT NULL,
    [NotifyKeyPeriod]     INT              CONSTRAINT [DF_LocalSettings_NotifyKeyPeriod] DEFAULT ((0)) NOT NULL,
    [LastSyncDate]        DATETIME         NULL,
    [LicenseExpiry]       DATETIME         NULL,
    [NotifyAdresses]      NVARCHAR (MAX)   NULL,
    [DefaultDivisionId]   UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_LocalSettings] PRIMARY KEY CLUSTERED ([Id] ASC)
);

