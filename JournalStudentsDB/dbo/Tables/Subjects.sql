CREATE TABLE [dbo].[Subjects] (
    [ID]      INT           IDENTITY (1, 1) NOT NULL,
    [Name]    NVARCHAR (50) NOT NULL,
    [ImageID] INT           NOT NULL,
    CONSTRAINT [PK_Subjects] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_Subjects_Images] FOREIGN KEY ([ImageID]) REFERENCES [dbo].[Images] ([ID])
);

