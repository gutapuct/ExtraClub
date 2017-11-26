CREATE TABLE [dbo].[Claims] (
    [Id]                UNIQUEIDENTIFIER CONSTRAINT [DF_Claims_Id] DEFAULT (newid()) NOT NULL,
    [CompanyId]         UNIQUEIDENTIFIER NOT NULL,
    [ClaimTypeId]       INT              NOT NULL,
    [CreatedBy]         UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn]         DATETIME         NOT NULL,
    [StatusDescription] NVARCHAR (1024)  NULL,
    [FtmId]             INT              NULL,
    [FinishDate]        DATETIME         NULL,
    [FinishedByName]    NVARCHAR (MAX)   NULL,
    [FinishedByFtmId]   UNIQUEIDENTIFIER NULL,
    [FinishDescription] NVARCHAR (MAX)   NULL,
    [ContactInfo]       NVARCHAR (MAX)   NULL,
    [ContactEmail]      NVARCHAR (MAX)   NULL,
    [ContactPhone]      NVARCHAR (MAX)   NULL,
    [Subject]           NVARCHAR (MAX)   NULL,
    [Message]           NVARCHAR (MAX)   NULL,
    [Eq_BuyDate]        NVARCHAR (MAX)   NULL,
    [Eq_Serial]         NVARCHAR (MAX)   NULL,
    [Eq_Guaranty]       NVARCHAR (MAX)   NULL,
    [PrefFinishDate]    NVARCHAR (MAX)   NULL,
    [StatusId]          INT              NOT NULL,
    [SubmitDate]        DATETIME         NULL,
    [SubmitUser]        UNIQUEIDENTIFIER NULL,
    [Circulation]       NVARCHAR (MAX)   NULL,
    [Eq_TreatmentId]    UNIQUEIDENTIFIER NULL,
    [Eq_TechContact]    NVARCHAR (MAX)   NULL,
    [Eq_SerialGutwell]  NVARCHAR (MAX)   NULL,
    [Eq_Model]          NVARCHAR (MAX)   NULL,
    [Eq_ClubAddr]       NVARCHAR (MAX)   NULL,
    [Eq_PostAddr]       NVARCHAR (MAX)   NULL,
    [ActualScore] INT NULL, 
    CONSTRAINT [PK_Claims] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
ALTER TABLE [dbo].[Claims] ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = OFF);

