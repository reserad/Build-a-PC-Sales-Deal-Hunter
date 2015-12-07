USE [db]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SendEmail_sps]') AND type in (N'P', N'PC'))
    exec('CREATE PROCEDURE [dbo].[SendEmail_sps] AS BEGIN SET NOCOUNT ON; END')
GO

ALTER PROCEDURE [dbo].[SendEmail_sps]
@Email varchar(50),
@URL varchar(200)

AS
BEGIN
    SET NOCOUNT ON;
    SELECT COUNT(*) FROM [dbo].[EmailsSent] WHERE [Email] = @Email AND [URL] = @URL
END