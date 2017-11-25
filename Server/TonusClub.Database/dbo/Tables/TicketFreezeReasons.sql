CREATE TABLE [dbo].[TicketFreezeReasons] (
    [Id]               UNIQUEIDENTIFIER CONSTRAINT [DF_TicketFreezeReasons_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [Name]             NVARCHAR (MAX)   NOT NULL,
    [CreatedOn]        DATETIME         CONSTRAINT [DF_TicketFreezeReasons_CreatedOn] DEFAULT (getdate()) NOT NULL,
    [CreatedBy]        UNIQUEIDENTIFIER NOT NULL,
    [IsBackDayAllowed] BIT              CONSTRAINT [DF_TicketFreezeReasons_IsBackDayAllowed] DEFAULT ((0)) NOT NULL,
    [K4]               MONEY            CONSTRAINT [DF_TicketFreezeReasons_K4] DEFAULT ((1)) NOT NULL,
    [IsReturnReason]   BIT              CONSTRAINT [DF_TicketFreezeReasons_IsReturnReason] DEFAULT ((0)) NOT NULL,
    [IsAvail]          BIT              CONSTRAINT [DF_TicketFreezeReasons_IsAvail] DEFAULT ((1)) NOT NULL,
    [NameEn]           NVARCHAR (256)   NULL,
    CONSTRAINT [PK_TicketFreezeReasons] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
ALTER TABLE [dbo].[TicketFreezeReasons] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

