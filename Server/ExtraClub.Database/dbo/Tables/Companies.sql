﻿CREATE TABLE [dbo].[Companies] (
    [CompanyId]                     UNIQUEIDENTIFIER NOT NULL,
    [CompanyName]                   NVARCHAR (256)   NOT NULL,
    [ConcessionNumber]              NVARCHAR (50)    NULL,
    [ResidualValueP1]               DECIMAL (18, 3)  CONSTRAINT [DF_Companies_ResidualValueP1] DEFAULT ((0.7)) NOT NULL,
    [ResidualValueP2]               DECIMAL (18, 3)  CONSTRAINT [DF_Companies_ResidualValueP2] DEFAULT ((0.3)) NOT NULL,
    [ResidualValueK11]              DECIMAL (18, 3)  CONSTRAINT [DF_Companies_ResidualValueK1] DEFAULT ((1)) NOT NULL,
    [ResidualValueK12]              DECIMAL (18, 3)  CONSTRAINT [DF_Companies_ResidualValueK2] DEFAULT ((0.75)) NOT NULL,
    [ResidualValueK13]              DECIMAL (18, 3)  CONSTRAINT [DF_Companies_ResidualValueK3] DEFAULT ((0.5)) NOT NULL,
    [ResidualValueK2]               DECIMAL (18, 3)  CONSTRAINT [DF_Companies_ResidualValueK2_1] DEFAULT ((1)) NOT NULL,
    [ResidualValueS1]               DECIMAL (18, 3)  CONSTRAINT [DF_Companies_ResidualValueS1] DEFAULT ((0)) NOT NULL,
    [CityName]                      NVARCHAR (250)   NULL,
    [GeneralManagerRod]             NVARCHAR (250)   NULL,
    [GeneralManagerBaseRod]         NVARCHAR (500)   NULL,
    [EssentialElements]             NVARCHAR (MAX)   NULL,
    [GeneralManagerName]            NVARCHAR (250)   NULL,
    [GeneralManagerPos]             NVARCHAR (250)   NULL,
    [MaxGuestUnits]                 INT              CONSTRAINT [DF_Companies_MaxGuestUnits] DEFAULT ((5)) NOT NULL,
    [TicketRebillCommission]        MONEY            CONSTRAINT [DF_Companies_TicketRebillCommission] DEFAULT ((500)) NOT NULL,
    [FreezePrice]                   MONEY            CONSTRAINT [DF_Companies_FreezePrice] DEFAULT ((0)) NOT NULL,
    [MaxCancellationPeriod]         INT              CONSTRAINT [DF_Companies_MaxCancellationPeriod] DEFAULT ((4)) NOT NULL,
    [TicketReturnPercentCommission] MONEY            CONSTRAINT [DF_Companies_TicketReturnPercentCommission] DEFAULT ((0)) NOT NULL,
    [TicketReturnFixedCommission]   MONEY            CONSTRAINT [DF_Companies_TicketReturnFixedCommission] DEFAULT ((0)) NOT NULL,
    [MaxTreatmentReserve]           INT              CONSTRAINT [DF_Companies_MaxTreatmentReserve] DEFAULT ((3)) NOT NULL,
    [HasSubway]                     BIT              CONSTRAINT [DF_Companies_HasSubway] DEFAULT ((1)) NOT NULL,
    [ShelfLostPenalty]              INT              CONSTRAINT [DF_Companies_ShelfLostPenalty] DEFAULT ((1)) NOT NULL,
    [SafeLostPenalty]               INT              CONSTRAINT [DF_Companies_SafeLostPenalty] DEFAULT ((1)) NOT NULL,
    [GeneralAccountantName]         NVARCHAR (250)   NULL,
    [DepositComissionPercent]       MONEY            CONSTRAINT [DF_Companies_DepositComissionPercent_1] DEFAULT ((0)) NOT NULL,
    [DepositComissionRub]           MONEY            CONSTRAINT [DF_Companies_DepositComissionRub_1] DEFAULT ((0)) NOT NULL,
    [DepositWarning]                NVARCHAR (250)   NULL,
    [CashWarning]                   NVARCHAR (250)   NULL,
    [CardWarning]                   NVARCHAR (250)   NULL,
    [IncomingCallText]              NVARCHAR (MAX)   NULL,
    [IncomingNewCusomerCallText]    NVARCHAR (MAX)   NULL,
    [IncomingNotACusomerCallText]   NVARCHAR (MAX)   NULL,
    [LostCutomerDays]               INT              CONSTRAINT [DF_Companies_LostCutomerDays] DEFAULT ((60)) NOT NULL,
    [GalloperId]                    NVARCHAR (MAX)   NULL,
    [OrgForm]                       NVARCHAR (MAX)   NULL,
    [INN]                           NVARCHAR (MAX)   NULL,
    [KPP]                           NVARCHAR (MAX)   NULL,
    [RSBank]                        NVARCHAR (MAX)   NULL,
    [KSBank]                        NVARCHAR (MAX)   NULL,
    [BankName]                      NVARCHAR (MAX)   NULL,
    [BIK]                           NVARCHAR (MAX)   NULL,
    [BankCity]                      NVARCHAR (MAX)   NULL,
    [Phone1]                        NVARCHAR (MAX)   NULL,
    [Phone2]                        NVARCHAR (MAX)   NULL,
    [MaxFreezeUnits]                INT              NULL,
    [MaxFreezePercent]              DECIMAL (18, 3)  NULL,
    [ReportEmail]                   NVARCHAR (256)   NULL,
    [UtcCorr]                       SMALLINT         CONSTRAINT [DF_Companies_UtcCorr] DEFAULT ((0)) NOT NULL,
    [Address]                       NVARCHAR (MAX)   NULL,
    [UserPrefix]                    NVARCHAR (6)     CONSTRAINT [DF_Companies_UserPrefix] DEFAULT ('') NOT NULL,
    [TicketsClubs]                  BIT              CONSTRAINT [DF_Companies_TicketsClubs] DEFAULT ((0)) NOT NULL,
    [AccountantName]                NVARCHAR (500)   NULL,
    [ActivateInstalment]            MONEY            CONSTRAINT [DF_Companies_ActivateInstalment] DEFAULT ((0)) NOT NULL,
    [CompanyVopsEmail]              NVARCHAR (400)   NULL,
    CONSTRAINT [PK_Companies] PRIMARY KEY CLUSTERED ([CompanyId] ASC)
);


GO
ALTER TABLE [dbo].[Companies] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

