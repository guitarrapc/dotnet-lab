USE master;
GO
IF DB_ID (N'test') IS NOT NULL
DROP DATABASE [test];
GO
CREATE DATABASE [test]
COLLATE Japanese_Bushu_Kakusu_140_CI_AS;
GO
