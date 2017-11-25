CREATE TABLE [dbo].[Tasks] (
    [Id]               UNIQUEIDENTIFIER CONSTRAINT [DF_Tasks_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [CompanyId]        UNIQUEIDENTIFIER NOT NULL,
    [CreatedBy]        UNIQUEIDENTIFIER NULL,
    [CreatedOn]        DATETIME         NOT NULL,
    [ExpiryOn]         DATETIME         NOT NULL,
    [Subject]          NVARCHAR (250)   NOT NULL,
    [Message]          NVARCHAR (MAX)   NOT NULL,
    [StatusId]         INT              NOT NULL,
    [Priority]         INT              NOT NULL,
    [ClosedBy]         UNIQUEIDENTIFIER NULL,
    [ClosedOn]         DATETIME         NULL,
    [ClosedComment]    NVARCHAR (MAX)   NULL,
    [Parameter]        UNIQUEIDENTIFIER NULL,
    [Eq_TreatmentId]   UNIQUEIDENTIFIER NULL,
    [Eq_TechContact]   NVARCHAR (MAX)   NULL,
    [Eq_SerialGutwell] NVARCHAR (MAX)   NULL,
    [Eq_Model]         NVARCHAR (MAX)   NULL,
    [Eq_ClubAddr]      NVARCHAR (MAX)   NULL,
    [Eq_PostAddr]      NVARCHAR (MAX)   NULL,
    CONSTRAINT [PK_Tasks] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Tasks_Companies] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Companies] ([CompanyId]),
    CONSTRAINT [FK_Tasks_Users] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users] ([UserId]),
    CONSTRAINT [FK_Tasks_Users1] FOREIGN KEY ([ClosedBy]) REFERENCES [dbo].[Users] ([UserId])
);


GO
ALTER TABLE [dbo].[Tasks] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);


GO
CREATE NONCLUSTERED INDEX [IX_CompanyId]
    ON [dbo].[Tasks]([CompanyId] ASC);

