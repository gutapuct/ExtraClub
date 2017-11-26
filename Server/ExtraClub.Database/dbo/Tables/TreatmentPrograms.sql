CREATE TABLE [dbo].[TreatmentPrograms] (
    [Id]               UNIQUEIDENTIFIER CONSTRAINT [DF_TreatmentPrograms_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [CompanyId]        UNIQUEIDENTIFIER NOT NULL,
    [CreatedBy]        UNIQUEIDENTIFIER NOT NULL,
    [ProgramName]      NVARCHAR (200)   NOT NULL,
    [NextProgramId]    UNIQUEIDENTIFIER NULL,
    [IsAvail]          BIT              CONSTRAINT [DF_TreatmentPrograms_IsAvail] DEFAULT ((1)) NOT NULL,
    [SettingsFolderId] UNIQUEIDENTIFIER NULL,
    [IsFixed]          BIT              CONSTRAINT [DF_TreatmentPrograms_IsFixed] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_TreatmentPrograms] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_TreatmentPrograms_Companies] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Companies] ([CompanyId]),
    CONSTRAINT [FK_TreatmentPrograms_TreatmentPrograms] FOREIGN KEY ([NextProgramId]) REFERENCES [dbo].[TreatmentPrograms] ([Id]),
    CONSTRAINT [FK_TreatmentPrograms_Users] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users] ([UserId])
);


GO
ALTER TABLE [dbo].[TreatmentPrograms] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

