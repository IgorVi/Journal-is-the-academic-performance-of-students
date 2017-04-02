CREATE TABLE [dbo].[Groups] (
    [ID]        INT           IDENTITY (1, 1) NOT NULL,
    [Name]      NVARCHAR (50) NOT NULL,
    [Admission] DATE          NOT NULL,
    CONSTRAINT [PK_Classes] PRIMARY KEY CLUSTERED ([ID] ASC)
);

