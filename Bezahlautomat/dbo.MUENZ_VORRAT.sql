CREATE TABLE [dbo].[MUENZ_VORRAT] (
    [MuenzTyp] INT NOT NULL PRIMARY KEY,
    [Anzahl]   INT DEFAULT ((20)) NOT NULL
);

INSERT INTO [dbo].[MUENZ_VORRAT] (MuenzTyp, Anzahl)
VALUES 
(0, 20),
(1, 20),
(2, 20),
(3, 20),
(4, 20),
(5, 20),
(6, 20),
(7, 20);