CREATE TABLE [dbo].[TextActionsDivisions] (
    [TextActionId] UNIQUEIDENTIFIER NOT NULL,
    [DivisionId]   UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_TextActionsDivisions] PRIMARY KEY CLUSTERED ([TextActionId] ASC, [DivisionId] ASC),
    CONSTRAINT [FK_TextActionsDivisions_Divisions] FOREIGN KEY ([DivisionId]) REFERENCES [dbo].[Divisions] ([Id]),
    CONSTRAINT [FK_TextActionsDivisions_TextActions] FOREIGN KEY ([TextActionId]) REFERENCES [dbo].[TextActions] ([Id])
);


GO
ALTER TABLE [dbo].[TextActionsDivisions] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

