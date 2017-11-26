CREATE TABLE [dbo].[OrganizationTypes] (
    [Id]         UNIQUEIDENTIFIER CONSTRAINT [DF_OrganizationType_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [Name]       NVARCHAR (250)   NOT NULL,
    [CreatedBy]  UNIQUEIDENTIFIER NOT NULL,
    [FirebirdId] INT              NULL,
    [IsAvail]    BIT              CONSTRAINT [DF_OrganizationTypes_IsAvail] DEFAULT ((1)) NOT NULL,
    [NameEn]     NVARCHAR (256)   NULL,
    CONSTRAINT [PK_OrganizationType] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
ALTER TABLE [dbo].[OrganizationTypes] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

