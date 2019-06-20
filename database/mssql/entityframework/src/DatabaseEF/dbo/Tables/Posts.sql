CREATE TABLE [dbo].[Posts] (
    [PostId]  INT            IDENTITY (1, 1) NOT NULL,
    [Title]   NVARCHAR (MAX) NULL,
    [Content] NVARCHAR (MAX) NULL,
    [BlogId]  INT            NOT NULL
);

