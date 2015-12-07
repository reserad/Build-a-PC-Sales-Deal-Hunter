USE [db]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Task_spd]') AND type in (N'P', N'PC'))
    exec('CREATE PROCEDURE [dbo].[Task_spd] AS BEGIN SET NOCOUNT ON; END')
GO

ALTER PROCEDURE [dbo].[Task_spd]
@Email varchar(50)

AS
BEGIN
    SET NOCOUNT ON;
	DELETE FROM [dbo].[EmailsSent] WHERE [Email] = @Email;
	DELETE FROM [dbo].[Emails] WHERE [Email] = @Email
END