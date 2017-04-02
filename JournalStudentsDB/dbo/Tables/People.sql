CREATE TABLE [dbo].[People] (
    [ID]          INT           IDENTITY (1, 1) NOT NULL,
    [LastName]    NVARCHAR (50) NOT NULL,
    [FirstName]   NVARCHAR (50) NOT NULL,
    [MiddleName]  NVARCHAR (50) NOT NULL,
    [Birthday]    DATE          NOT NULL,
    [PhoneNumber] NVARCHAR (20) NULL,
    [Email]       NVARCHAR (50) NULL,
    [ImageID]     INT           NOT NULL,
    [Address]     NVARCHAR (50) NULL,
    CONSTRAINT [PK_People] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_People_Images] FOREIGN KEY ([ImageID]) REFERENCES [dbo].[Images] ([ID])
);

