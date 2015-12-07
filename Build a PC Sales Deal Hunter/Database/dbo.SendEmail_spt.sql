USE [db]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SendEmail_spt]') AND type in (N'P', N'PC'))
    exec('CREATE PROCEDURE [dbo].[SendEmail_spt] AS BEGIN SET NOCOUNT ON; END')
GO

ALTER PROCEDURE [dbo].[SendEmail_spt]
@Email varchar(50),
@URL varchar(200)

AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO [dbo].[EmailsSent] ([URL], [Email]) VALUES (@URL, @Email)
END