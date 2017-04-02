CREATE TABLE [dbo].[Teachers] (
    [ID]       INT IDENTITY (1, 1) NOT NULL,
    [PeopleID] INT NOT NULL,
    CONSTRAINT [PK_Teachers] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_Teachers_People] FOREIGN KEY ([PeopleID]) REFERENCES [dbo].[People] ([ID])
);

