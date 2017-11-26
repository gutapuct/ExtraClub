CREATE TABLE [dbo].[ContraIndications] (
    [Id]        UNIQUEIDENTIFIER CONSTRAINT [DF_ContraIndications_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [Name]      NVARCHAR (150)   NOT NULL,
    [IsVisible] BIT              CONSTRAINT [DF_ContraIndications_IsVisible] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_ContraIndications] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
ALTER TABLE [dbo].[ContraIndications] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

