CREATE TABLE [dbo].[SmsMessages] (
    [Id]         UNIQUEIDENTIFIER CONSTRAINT [DF_SmsMessages_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [CustomerId] UNIQUEIDENTIFIER NOT NULL,
    [ToSendFrom] DATETIME         NOT NULL,
    [SentOn]     DATETIME         NULL,
    [Phone]      NVARCHAR (20)    NOT NULL,
    [Text]       NVARCHAR (500)   NOT NULL,
    [Report]     NVARCHAR (MAX)   NULL,
    [SkipCheck]  BIT              CONSTRAINT [DF_SmsMessages_SkipCheck] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_SmsMessages] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_CustomerId]
    ON [dbo].[SmsMessages]([CustomerId] ASC)
    INCLUDE([ToSendFrom], [SentOn], [Text]);

