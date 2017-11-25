CREATE TABLE [dbo].[Customers] (
    [Id]                  UNIQUEIDENTIFIER CONSTRAINT [DF_Customers_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [CompanyId]           UNIQUEIDENTIFIER CONSTRAINT [DF_Customers_CompanyId] DEFAULT (newid()) NOT NULL,
    [FirstName]           NVARCHAR (250)   NULL,
    [MiddleName]          NVARCHAR (250)   NULL,
    [LastName]            NVARCHAR (250)   NULL,
    [Birthday]            DATETIME         NULL,
    [Gender]              BIT              NOT NULL,
    [AdvertTypeId]        UNIQUEIDENTIFIER NULL,
    [Phone1]              NVARCHAR (250)   NULL,
    [Phone2]              NVARCHAR (250)   NULL,
    [Email]               VARCHAR (512)    NULL,
    [Job]                 NVARCHAR (128)   NULL,
    [Comments]            NVARCHAR (512)   NULL,
    [IsActive]            BIT              CONSTRAINT [DF_Customers_IsActive] DEFAULT ((1)) NOT NULL,
    [StatusId]            UNIQUEIDENTIFIER NULL,
    [MailingList]         BIT              NULL,
    [ClubId]              UNIQUEIDENTIFIER NULL,
    [CreatedOn]           DATETIME         CONSTRAINT [DF_Customers_CreatedOn] DEFAULT (getdate()) NOT NULL,
    [ModifiedOn]          DATETIME         NULL,
    [AuthorId]            UNIQUEIDENTIFIER NOT NULL,
    [tmp]                 NVARCHAR (250)   NULL,
    [IsEmployee]          BIT              CONSTRAINT [DF_Customers_IsEmployee] DEFAULT ((0)) NOT NULL,
    [AddrIndex]           NVARCHAR (250)   NULL,
    [AddrCity]            NVARCHAR (250)   NULL,
    [AddrStreet]          NVARCHAR (250)   NULL,
    [AddrOther]           NVARCHAR (250)   NULL,
    [AddrMetro]           NVARCHAR (250)   NULL,
    [PasspNumber]         NVARCHAR (250)   NULL,
    [PasspEmitPlace]      NVARCHAR (250)   NULL,
    [PasspEmitDate]       SMALLDATETIME    NULL,
    [SmsList]             BIT              CONSTRAINT [DF_Customers_SmsList] DEFAULT ((1)) NOT NULL,
    [AdvertComment]       NVARCHAR (250)   NULL,
    [InvitorId]           UNIQUEIDENTIFIER NULL,
    [NoContraIndications] BIT              NULL,
    [CorporateId]         UNIQUEIDENTIFIER NULL,
    [SocialStatusId]      INT              NULL,
    [WorkPlace]           NVARCHAR (512)   NULL,
    [Position]            NVARCHAR (512)   NULL,
    [WorkPhone]           NVARCHAR (512)   NULL,
    [Kids]                INT              NULL,
    [Image]               VARBINARY (MAX)  NULL,
    [Password]            NVARCHAR (128)   NULL,
    [FromSite]            BIT              CONSTRAINT [DF_Customers_FromSite] DEFAULT ((0)) NOT NULL,
    [MarketingPassed]     BIT              CONSTRAINT [DF_Customers_MarketingPassed] DEFAULT ((0)) NOT NULL,
    [IsWork]              BIT              NULL,
    [HasEmail]            BIT              NULL,
    [ManagerId] UNIQUEIDENTIFIER NULL, 
    CONSTRAINT [PK_Customers] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Customer_Companies] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Companies] ([CompanyId]),
    CONSTRAINT [FK_Customers_AdvertTypes] FOREIGN KEY ([AdvertTypeId]) REFERENCES [dbo].[AdvertTypes] ([Id]),
	CONSTRAINT [FK_Customers_Employees] FOREIGN KEY ([ManagerId]) REFERENCES [dbo].[Employees] ([Id]),
    CONSTRAINT [FK_Customers_Corporates] FOREIGN KEY ([CorporateId]) REFERENCES [dbo].[Corporates] ([Id]),
    CONSTRAINT [FK_Customers_Customers] FOREIGN KEY ([InvitorId]) REFERENCES [dbo].[Customers] ([Id]),
    CONSTRAINT [FK_UserCustomer] FOREIGN KEY ([AuthorId]) REFERENCES [dbo].[Users] ([UserId])
);


GO
ALTER TABLE [dbo].[Customers] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);


GO
CREATE NONCLUSTERED INDEX [IX_FK_Customer_Companies]
    ON [dbo].[Customers]([CompanyId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_FK_UserCustomer]
    ON [dbo].[Customers]([AuthorId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_SMS_2]
    ON [dbo].[Customers]([ClubId] ASC, [SmsList] ASC)
    INCLUDE([Id], [CompanyId], [Birthday], [Phone2]);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedOn]
    ON [dbo].[Customers]([CreatedOn] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ClubId]
    ON [dbo].[Customers]([ClubId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_InvitorId]
    ON [dbo].[Customers]([InvitorId] ASC);

