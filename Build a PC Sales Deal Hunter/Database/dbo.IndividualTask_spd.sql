USE [db]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[IndividualTask_spd]') AND type in (N'P', N'PC'))
    exec('CREATE PROCEDURE [dbo].[IndividualTask_spd] AS BEGIN SET NOCOUNT ON; END')
GO

ALTER PROCEDURE [dbo].[IndividualTask_spd]
@Email varchar(50),
@Query varchar(50),
@URL varchar(200),
@Price int

AS
BEGIN
    SET NOCOUNT ON;
	DELETE FROM [dbo].[EmailsSent] WHERE [Email] = @Email AND [URL] = @URL;
	DELETE FROM [dbo].[Emails] WHERE [Email] = @Email AND [Price] = @Price AND [Query] = @Query
END