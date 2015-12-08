USE [db]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[IndividualTask_sps]') AND type in (N'P', N'PC'))
    exec('CREATE PROCEDURE [dbo].[IndividualTask_sps] AS BEGIN SET NOCOUNT ON; END')
GO

ALTER PROCEDURE [dbo].[IndividualTask_sps]
@Email varchar(50)
AS
BEGIN
    SET NOCOUNT ON;
SELECT * FROM [dbo].[Emails] WHERE [Email] = @Email
END