CREATE TABLE [dbo].[Evaluation] (
    [ID]         INT IDENTITY (1, 1) NOT NULL,
    [Evaluation] INT NOT NULL,
    [StudentID]  INT NOT NULL,
    [LessonID]   INT NOT NULL,
    CONSTRAINT [PK_Evaluation] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_Evaluation_Lessons] FOREIGN KEY ([LessonID]) REFERENCES [dbo].[Lessons] ([ID]),
    CONSTRAINT [FK_Evaluation_Students] FOREIGN KEY ([StudentID]) REFERENCES [dbo].[Students] ([ID])
);

