CREATE TABLE [dbo].[Lessons] (
    [ID]        INT      IDENTITY (1, 1) NOT NULL,
    [SubjectID] INT      NOT NULL,
    [TeacherID] INT      NOT NULL,
    [GroupID]   INT      NOT NULL,
    [DateTime]  DATETIME NOT NULL,
    CONSTRAINT [PK_Lessons] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_Lessons_Ggroups] FOREIGN KEY ([GroupID]) REFERENCES [dbo].[Groups] ([ID]),
    CONSTRAINT [FK_Lessons_Subjects] FOREIGN KEY ([SubjectID]) REFERENCES [dbo].[Subjects] ([ID]),
    CONSTRAINT [FK_Lessons_Teachers] FOREIGN KEY ([TeacherID]) REFERENCES [dbo].[Teachers] ([ID])
);

