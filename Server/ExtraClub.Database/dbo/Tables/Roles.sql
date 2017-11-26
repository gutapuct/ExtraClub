CREATE TABLE [dbo].[Roles] (
    [RoleId]           UNIQUEIDENTIFIER NOT NULL,
    [RoleName]         NVARCHAR (256)   NOT NULL,
    [CreatedOn]        DATETIME         NOT NULL,
    [ModifiedOn]       DATETIME         NULL,
    [CompanyId]        UNIQUEIDENTIFIER CONSTRAINT [DF_Roles_CompanyId] DEFAULT ('66202163-0299-47B3-B89F-8DCD9CCD44C0') NOT NULL,
    [CardDiscs]        NVARCHAR (4000)  NULL,
    [TicketDiscs]      NVARCHAR (4000)  NULL,
    [IsReadonly]       BIT              CONSTRAINT [DF_Roles_IsReadonly] DEFAULT ((0)) NOT NULL,
    [SettingsFolderId] UNIQUEIDENTIFIER NULL,
    [CreatedBy]        UNIQUEIDENTIFIER NULL,
    [ModifiedBy]       UNIQUEIDENTIFIER NULL,
    [IsFixed]          BIT              CONSTRAINT [DF_Roles_IsFixed] DEFAULT ((0)) NOT NULL,
    [TicketRubDiscs]   NVARCHAR (4000)  NULL,
    CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED ([RoleId] ASC),
    CONSTRAINT [FK_Roles_Companies] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Companies] ([CompanyId])
);


GO
ALTER TABLE [dbo].[Roles] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

