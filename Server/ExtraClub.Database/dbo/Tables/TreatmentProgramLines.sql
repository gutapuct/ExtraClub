CREATE TABLE [dbo].[TreatmentProgramLines] (
    [Id]                 UNIQUEIDENTIFIER CONSTRAINT [DF_TreatmentProgramLines_Id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [CompanyId]          UNIQUEIDENTIFIER NOT NULL,
    [TreatmentProgramId] UNIQUEIDENTIFIER NOT NULL,
    [Position]           TINYINT          NOT NULL,
    [TreatmentConfigId]  UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_TreatmentProgramLines] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_TreatmentProgramLines_Companies] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Companies] ([CompanyId]),
    CONSTRAINT [FK_TreatmentProgramLines_TreatmentConfig] FOREIGN KEY ([TreatmentConfigId]) REFERENCES [dbo].[TreatmentConfig] ([Id]),
    CONSTRAINT [FK_TreatmentProgramLines_TreatmentPrograms] FOREIGN KEY ([TreatmentProgramId]) REFERENCES [dbo].[TreatmentPrograms] ([Id])
);


GO
ALTER TABLE [dbo].[TreatmentProgramLines] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

