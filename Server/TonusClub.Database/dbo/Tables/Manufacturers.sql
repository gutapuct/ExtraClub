CREATE TABLE [dbo].[Manufacturers] (
    [ManufacturerId]   UNIQUEIDENTIFIER NOT NULL,
    [ManufacturerName] NVARCHAR (MAX)   NOT NULL,
    [ModifiedOn]       DATETIME         NULL,
    [CreatedOn]        DATETIME         NOT NULL,
    [AuthorId]         UNIQUEIDENTIFIER NOT NULL,
    [CompanyId]        UNIQUEIDENTIFIER NOT NULL,
    [FirebirdId]       INT              NULL,
    [IsAvail]          BIT              CONSTRAINT [DF_Manufacturers_IsAvail] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_Manufacturers] PRIMARY KEY CLUSTERED ([ManufacturerId] ASC),
    CONSTRAINT [FK_ManufacturerCompany] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Companies] ([CompanyId])
);


GO
ALTER TABLE [dbo].[Manufacturers] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);


GO
CREATE NONCLUSTERED INDEX [IX_FK_UserManufacturer]
    ON [dbo].[Manufacturers]([AuthorId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_FK_ManufacturerCompany]
    ON [dbo].[Manufacturers]([CompanyId] ASC);

