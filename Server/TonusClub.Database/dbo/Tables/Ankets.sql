﻿CREATE TABLE [dbo].[Ankets] (
    [Id]                    UNIQUEIDENTIFIER CONSTRAINT [DF_Ankets_Id] DEFAULT (newid()) NOT NULL,
    [CompanyId]             UNIQUEIDENTIFIER NOT NULL,
    [CreatedBy]             UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn]             DATETIME         NOT NULL,
    [Period]                DATETIME         NOT NULL,
    [DivisionId]            UNIQUEIDENTIFIER NOT NULL,
    [StatusId]              INT              NOT NULL,
    [PriceChanges]          BIT              NOT NULL,
    [TotalWorkdays]         INT              NOT NULL,
    [AvgVisitors]           MONEY            NOT NULL,
    [AvgTreatments]         MONEY            NOT NULL,
    [TotalTestVisitors]     INT              NOT NULL,
    [TotalBuyAfterTest]     INT              NOT NULL,
    [AdvertSpendings]       MONEY            NOT NULL,
    [NetActions]            NVARCHAR (MAX)   NULL,
    [HadSelfActions]        BIT              NOT NULL,
    [SelfActions]           NVARCHAR (MAX)   NULL,
    [NextNetActions]        NVARCHAR (MAX)   NULL,
    [RecurringTickets]      INT              NOT NULL,
    [PlanComplete]          MONEY            NOT NULL,
    [TotalCash]             MONEY            NOT NULL,
    [EmployeesGrade]        INT              NULL,
    [EmployeesGradeDesc]    NVARCHAR (MAX)   NULL,
    [EmployeesChange]       BIT              NOT NULL,
    [Meeting]               BIT              NOT NULL,
    [MeetingDesc]           NVARCHAR (MAX)   NULL,
    [Test]                  BIT              NOT NULL,
    [TestDesc]              NVARCHAR (MAX)   NULL,
    [NewTreatments]         BIT              NOT NULL,
    [NewTreatmentsDesc]     NVARCHAR (MAX)   NULL,
    [TreatmentProblems]     BIT              NOT NULL,
    [TreatmentProblemsDesc] NVARCHAR (MAX)   NULL,
    [ClubInfo]              BIT              NOT NULL,
    [ClubInfoDesc]          NVARCHAR (MAX)   NULL,
    [ClubNews]              BIT              NOT NULL,
    [ClubNewsDesc]          NVARCHAR (MAX)   NULL,
    [ClubDevGrade]          INT              NULL,
    [ClubDevDesc]           NVARCHAR (MAX)   NULL,
    [SelfGrade]             INT              NULL,
    [SelfDesc]              NVARCHAR (MAX)   NULL,
    [FranchGrade]           INT              NULL,
    [FranchDesc]            NVARCHAR (MAX)   NULL,
    [FranchSuppGrade]       INT              NULL,
    [FranchSuppDesc]        NVARCHAR (MAX)   NULL,
    [AsuSuppGrade]          INT              NULL,
    [AsuSuppDesc]           NVARCHAR (MAX)   NULL,
    [DesignerGrade]         INT              NULL,
    [DesignerDesc]          NVARCHAR (MAX)   NULL,
    [SiteAdmGrade]          INT              NULL,
    [SiteAdmDesc]           NVARCHAR (MAX)   NULL,
    [AccountantsGrade]      INT              NULL,
    [AccountantsDesc]       NVARCHAR (MAX)   NULL,
    [LogistGrade]           INT              NULL,
    [LogistDesc]            NVARCHAR (MAX)   NULL,
    [RepairGrade]           INT              NULL,
    [RepairDesc]            NVARCHAR (MAX)   NULL,
    [BeautyNatureGrade]     INT              NULL,
    [BeautyNatureDesc]      NVARCHAR (MAX)   NULL,
    [BeInformedGrade]       INT              NULL,
    [BeInformedDesc]        NVARCHAR (MAX)   NULL,
    [IncomeFactors]         NVARCHAR (MAX)   NULL,
    [Wishes]                NVARCHAR (MAX)   NULL,
    [FilledBy]              NVARCHAR (MAX)   NULL,
    [FilledByPosition]      NVARCHAR (MAX)   NULL,
    [SentDate]              DATETIME         NULL,
    [OlorantiFood]          NVARCHAR (500)   NULL,
    [OlorantiCosm]          NVARCHAR (500)   NULL,
    [HomeEquipment]         NVARCHAR (500)   NULL,
    CONSTRAINT [PK_Ankets] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
ALTER TABLE [dbo].[Ankets] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);


GO
CREATE NONCLUSTERED INDEX [IX_For_Crm]
    ON [dbo].[Ankets]([DivisionId] ASC);
