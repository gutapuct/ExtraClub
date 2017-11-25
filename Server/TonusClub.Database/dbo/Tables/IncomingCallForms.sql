CREATE TABLE [dbo].[IncomingCallForms] (
    [Id]          UNIQUEIDENTIFIER ROWGUIDCOL NOT NULL,
    [FormText]    NVARCHAR (MAX)   NOT NULL,
    [HasInputBox] BIT              NOT NULL,
    [IsStartForm] BIT              NOT NULL,
    [Header]      NVARCHAR (256)   NOT NULL,
    CONSTRAINT [PK_IncomingCallForms] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
ALTER TABLE [dbo].[IncomingCallForms] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

