CREATE TABLE [dbo].[Students] (
    [ID]       INT IDENTITY (1, 1) NOT NULL,
    [PeopleID] INT NOT NULL,
    [GroupID]  INT NOT NULL,
    CONSTRAINT [PK_Students] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_Students_Ggroups] FOREIGN KEY ([GroupID]) REFERENCES [dbo].[Groups] ([ID]),
    CONSTRAINT [FK_Students_People] FOREIGN KEY ([PeopleID]) REFERENCES [dbo].[People] ([ID])
);

