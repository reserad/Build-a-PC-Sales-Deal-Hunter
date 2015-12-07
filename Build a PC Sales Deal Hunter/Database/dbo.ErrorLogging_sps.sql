USE [db]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ErrorLogging_sps]') AND type in (N'P', N'PC'))
    exec('CREATE PROCEDURE [dbo].[ErrorLogging_sps] AS BEGIN SET NOCOUNT ON; END')
GO

ALTER PROCEDURE [dbo].[ErrorLogging_sps]
AS
BEGIN
    SET NOCOUNT ON;
	SELECT * FROM [dbo].[ErrorLogging]
END