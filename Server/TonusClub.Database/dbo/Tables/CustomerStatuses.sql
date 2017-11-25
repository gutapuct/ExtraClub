CREATE TABLE [dbo].[CustomerStatuses] (
    [Id]        UNIQUEIDENTIFIER CONSTRAINT [DF_CustomerStatuses_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [Name]      NVARCHAR (150)   NOT NULL,
    [IsAvail]   BIT              CONSTRAINT [DF_CustomerStatuses_IsAvail] DEFAULT ((1)) NOT NULL,
    [CompanyId] UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_CustomerStatuses] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
ALTER TABLE [dbo].[CustomerStatuses] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

