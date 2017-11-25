CREATE TABLE [dbo].[Anthropometrics] (
    [Id]           UNIQUEIDENTIFIER CONSTRAINT [DF_Anthropometrics_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [CustomerId]   UNIQUEIDENTIFIER NOT NULL,
    [CompanyId]    UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn]    DATETIME         NOT NULL,
    [CreatedBy]    UNIQUEIDENTIFIER NOT NULL,
    [Height]       NVARCHAR (MAX)   NULL,
    [Weight]       NVARCHAR (MAX)   NULL,
    [PSPulse]      NVARCHAR (MAX)   NULL,
    [ADUp]         NVARCHAR (MAX)   NULL,
    [ADDown]       NVARCHAR (MAX)   NULL,
    [Neck]         NVARCHAR (MAX)   NULL,
    [ChestIn]      NVARCHAR (MAX)   NULL,
    [ChestOut]     NVARCHAR (MAX)   NULL,
    [Shoulders]    NVARCHAR (MAX)   NULL,
    [RightRelax]   NVARCHAR (MAX)   NULL,
    [RightTense]   NVARCHAR (MAX)   NULL,
    [LeftRelax]    NVARCHAR (MAX)   NULL,
    [LeftTense]    NVARCHAR (MAX)   NULL,
    [ForearmRight] NVARCHAR (MAX)   NULL,
    [ForearmLeft]  NVARCHAR (MAX)   NULL,
    [Waist]        NVARCHAR (MAX)   NULL,
    [Stomach]      NVARCHAR (MAX)   NULL,
    [Leg]          NVARCHAR (MAX)   NULL,
    [Buttocks]     NVARCHAR (MAX)   NULL,
    [LegRight]     NVARCHAR (MAX)   NULL,
    [LegLeft]      NVARCHAR (MAX)   NULL,
    [ShinRight]    NVARCHAR (MAX)   NULL,
    [ShinLeft]     NVARCHAR (MAX)   NULL,
    [Fat]          NVARCHAR (MAX)   NULL,
    [InternalMass] NVARCHAR (MAX)   NULL,
    [MusculeMass]  NVARCHAR (MAX)   NULL,
    [Water]        NVARCHAR (MAX)   NULL,
    [BonesMass]    NVARCHAR (MAX)   NULL,
    [KkalBurn]     NVARCHAR (MAX)   NULL,
    [MetaAge]      NVARCHAR (MAX)   NULL,
    [VascFat]      NVARCHAR (MAX)   NULL,
    [MassIndex]    NVARCHAR (MAX)   NULL,
    [IdealWeight]  NVARCHAR (MAX)   NULL,
    [FatStage]     NVARCHAR (MAX)   NULL,
    CONSTRAINT [PK_Anthropometrics] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Anthropometrics_Companies] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Companies] ([CompanyId]),
    CONSTRAINT [FK_Anthropometrics_Customers] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customers] ([Id]),
    CONSTRAINT [FK_Anthropometrics_Users] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users] ([UserId])
);


GO
ALTER TABLE [dbo].[Anthropometrics] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);


GO
CREATE NONCLUSTERED INDEX [IX_Company]
    ON [dbo].[Anthropometrics]([CompanyId] ASC);

