USE [db]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ErrorLogging_spt]') AND type in (N'P', N'PC'))
    exec('CREATE PROCEDURE [dbo].[ErrorLogging_spt] AS BEGIN SET NOCOUNT ON; END')
GO

ALTER PROCEDURE [dbo].[ErrorLogging_spt]
@Error varchar(50),
@Time varchar(200)

AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO [dbo].[ErrorLogging] ([Error], [Time]) VALUES (@Error, @Time)
END