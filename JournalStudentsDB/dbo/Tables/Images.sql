CREATE TABLE [dbo].[Images] (
    [ID]    INT             IDENTITY (1, 1) NOT NULL,
    [Image] VARBINARY (MAX) NOT NULL,
    CONSTRAINT [PK_Images] PRIMARY KEY CLUSTERED ([ID] ASC)
);

