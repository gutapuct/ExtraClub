CREATE TABLE [dbo].[RecurrentSentInfos] (
    [Id]       UNIQUEIDENTIFIER CONSTRAINT [DF_RecurrentSentInfos_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [RuleId]   UNIQUEIDENTIFIER NOT NULL,
    [SentDate] DATETIME         NOT NULL,
    CONSTRAINT [PK_RecurrentSentInfos] PRIMARY KEY CLUSTERED ([Id] ASC)
);

