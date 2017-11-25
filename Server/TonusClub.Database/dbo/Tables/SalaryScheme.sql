CREATE TABLE [dbo].[SalaryScheme] (
    [Id]             UNIQUEIDENTIFIER CONSTRAINT [DF_SalaryScheme_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [CompanyId]      UNIQUEIDENTIFIER NOT NULL,
    [DivisionId]     UNIQUEIDENTIFIER NOT NULL,
    [CreatedBy]      UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn]      DATETIME         NOT NULL,
    [Name]           NVARCHAR (250)   NOT NULL,
    [IsOvertimePaid] BIT              NOT NULL,
    [Late1Minutes]   INT              NULL,
    [Late1Fine]      MONEY            NULL,
    [Late2Minutes]   INT              NULL,
    [Late2Fine]      MONEY            NULL,
    [IsAvail]        BIT              CONSTRAINT [DF_SalaryScheme_IsAvail] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_SalaryScheme] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_SalaryScheme_Divisions] FOREIGN KEY ([DivisionId]) REFERENCES [dbo].[Divisions] ([Id]),
    CONSTRAINT [FK_SalaryScheme_Users] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users] ([UserId])
);


GO
ALTER TABLE [dbo].[SalaryScheme] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

