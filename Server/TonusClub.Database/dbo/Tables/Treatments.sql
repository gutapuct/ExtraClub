CREATE TABLE [dbo].[Treatments] (
    [Id]               UNIQUEIDENTIFIER CONSTRAINT [DF_Treatments_Id_1] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [Tag]              NVARCHAR (250)   NULL,
    [CompanyId]        UNIQUEIDENTIFIER NOT NULL,
    [DivisionId]       UNIQUEIDENTIFIER NOT NULL,
    [CreatedBy]        UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn]        DATETIME         NOT NULL,
    [IsActive]         BIT              CONSTRAINT [DF_Treatments_IsActive] DEFAULT ((1)) NOT NULL,
    [Comment]          NVARCHAR (512)   NULL,
    [ThreatmentTypeId] UNIQUEIDENTIFIER NOT NULL,
    [MaxCustomers]     INT              CONSTRAINT [DF_Treatments_MaximumCustomers] DEFAULT ((1)) NOT NULL,
    [DogNumber]        NVARCHAR (MAX)   NULL,
    [SerialNumber]     NVARCHAR (MAX)   NULL,
    [Delivery]         NVARCHAR (MAX)   NULL,
    [GuaranteeExp]     NVARCHAR (MAX)   NULL,
    [UseExp]           NVARCHAR (MAX)   NULL,
    [SettingsFolderId] UNIQUEIDENTIFIER NULL,
    [MacAddress]       NVARCHAR (MAX)   NULL,
    [UseController]    BIT              DEFAULT ((0)) NOT NULL,
    [IsOnline]         BIT              DEFAULT ((0)) NOT NULL,
    [Cost]             MONEY            NULL,
    [Order]            INT              CONSTRAINT [DF_Treatments_Order] DEFAULT ((0)) NOT NULL,
    [ModelName]        NVARCHAR (500)   NULL,
    CONSTRAINT [PK_Treatments_1] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Treatments_Divisions] FOREIGN KEY ([DivisionId]) REFERENCES [dbo].[Divisions] ([Id]),
    CONSTRAINT [FK_Treatments_Treatments] FOREIGN KEY ([Id]) REFERENCES [dbo].[Treatments] ([Id]),
    CONSTRAINT [FK_Treatments_TreatmentTypes] FOREIGN KEY ([ThreatmentTypeId]) REFERENCES [dbo].[TreatmentTypes] ([Id]),
    CONSTRAINT [FK_Treatments_Users] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users] ([UserId])
);


GO
ALTER TABLE [dbo].[Treatments] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);


GO
CREATE NONCLUSTERED INDEX [IX_Treatments]
    ON [dbo].[Treatments]([ThreatmentTypeId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Treatments_1]
    ON [dbo].[Treatments]([DivisionId] ASC);

