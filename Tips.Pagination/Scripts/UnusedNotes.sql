
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

USE master
GO

-- Create the database and tables
DROP DATABASE IF EXISTS DeveloperTipsPagination
GO

CREATE DATABASE DeveloperTipsPagination
GO

-----------------------
--
-- WARNING!!!
--
-- If you are using Azure database, then you'll need to manually connect to the new database.
-- https://feedback.azure.com/forums/217321-sql-database/suggestions/14822082-allow-the-use-statement-to-switch-between-database
--
-- WARNING!!!
--
-----------------------
--USE DeveloperTipsPagination
--GO


-- I need many records to make this worthwhile, so I'm using the adventureworks 2016 database.
-- https://www.microsoft.com/en-us/download/details.aspx?id=49502
-- Click Download
-- In SQL Server Management Studio (SSMS) AdventureWorks2016CTP3.bak

--CREATE TABLE [dbo].[Product](
--	[ProductId] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
--	[Name] [varchar](max) NOT NULL,
--	[EffectiveDate] [date] NOT NULL,
--	[ExpirationDate] [date] NULL
--)
--GO

--CREATE TABLE [dbo].[ProductSummary](
--	[ProductId] [int] NOT NULL PRIMARY KEY,
--	[Summary] [text] NULL,
--	CONSTRAINT [FK_ProductSummary_Product] FOREIGN KEY([ProductId])
--		REFERENCES [dbo].[Product] ([ProductId])
--)
--GO
