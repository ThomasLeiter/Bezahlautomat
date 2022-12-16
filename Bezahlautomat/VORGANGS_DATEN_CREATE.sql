CREATE TABLE [dbo].[VORGANGS_DATEN] (
    [Id]           INT      IDENTITY (1, 1) NOT NULL,
    [Datum]        DATETIME NULL,
    [BetragInCent] INT      DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

