CREATE TABLE [dbo].[AdvertTypes] (
    [Id]            UNIQUEIDENTIFIER CONSTRAINT [DF_AdvertTypes_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [Name]          NVARCHAR (250)   NOT NULL,
    [CommentNeeded] BIT              CONSTRAINT [DF_AdvertTypes_CommentNeeded] DEFAULT ((0)) NOT NULL,
    [InvitorNeeded] BIT              CONSTRAINT [DF_AdvertTypes_InvitorNeeded] DEFAULT ((0)) NOT NULL,
    [IsAvail]       BIT              CONSTRAINT [DF_AdvertTypes_IsAvail] DEFAULT ((1)) NOT NULL,
    [AdvertGroupId] UNIQUEIDENTIFIER CONSTRAINT [DF_AdvertTypes_AdvertGroupId] DEFAULT ('80B9951D-C21B-451E-8EC8-C0C3F9FD4051') NOT NULL,
    [CompanyId]     UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_AdvertTypes] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AdvertTypes_AdvertGroups] FOREIGN KEY ([AdvertGroupId]) REFERENCES [dbo].[AdvertGroups] ([Id])
);


GO
ALTER TABLE [dbo].[AdvertTypes] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

