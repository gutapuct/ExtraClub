CREATE TABLE [dbo].[Users] (
    [UserId]              UNIQUEIDENTIFIER NOT NULL,
    [UserName]            NVARCHAR (50)    NOT NULL,
    [FullName]            NVARCHAR (256)   NULL,
    [CompanyId]           UNIQUEIDENTIFIER NOT NULL,
    [PasswordHash]        VARCHAR (64)     NULL,
    [IsActive]            BIT              NOT NULL,
    [CreatedOn]           DATETIME         NOT NULL,
    [ModifiedOn]          DATETIME         NULL,
    [EmployeeId]          UNIQUEIDENTIFIER NULL,
    [LastLoginDate]       DATETIME         CONSTRAINT [DF_Users_LastLoginDate] DEFAULT (getdate()) NOT NULL,
    [LastPasswordChanged] DATETIME         NULL,
    [Email]               NVARCHAR (256)   NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([UserId] ASC),
    CONSTRAINT [FK_Users_Companies] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Companies] ([CompanyId])
);


GO
ALTER TABLE [dbo].[Users] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);


GO
CREATE NONCLUSTERED INDEX [IX_FK_Users_Companies]
    ON [dbo].[Users]([CompanyId] ASC);

