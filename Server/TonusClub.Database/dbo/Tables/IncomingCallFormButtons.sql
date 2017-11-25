CREATE TABLE [dbo].[IncomingCallFormButtons] (
    [Id]                 UNIQUEIDENTIFIER CONSTRAINT [DF_IncomingCallFormButtons_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [IncomingCallFormId] UNIQUEIDENTIFIER NOT NULL,
    [ButtonText]         NVARCHAR (250)   NOT NULL,
    [ButtonAction]       INT              NOT NULL,
    [Parameter]          UNIQUEIDENTIFIER NULL,
    [IsFinal]            BIT              CONSTRAINT [DF_IncomingCallFormButtons_IsFinal] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_IncomingCallFormButtons] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_IncomingCallFormButtons_IncomingCallForms] FOREIGN KEY ([IncomingCallFormId]) REFERENCES [dbo].[IncomingCallForms] ([Id])
);


GO
ALTER TABLE [dbo].[IncomingCallFormButtons] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

