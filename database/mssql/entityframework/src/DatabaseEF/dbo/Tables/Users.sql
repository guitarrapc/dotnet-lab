CREATE TABLE [dbo].[Users] (
    [UserId]  INT            IDENTITY (1, 1) NOT NULL,
    [Name]    NVARCHAR (MAX) NULL,
    [Created] DATETIMEOFFSET  NOT NULL
);

