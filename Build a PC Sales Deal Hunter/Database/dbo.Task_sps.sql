USE [db]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Task_sps]') AND type in (N'P', N'PC'))
    exec('CREATE PROCEDURE [dbo].[Task_sps] AS BEGIN SET NOCOUNT ON; END')
GO

ALTER PROCEDURE [dbo].[Task_sps]

AS
BEGIN
    SET NOCOUNT ON;
SELECT * FROM [dbo].[Emails]
END