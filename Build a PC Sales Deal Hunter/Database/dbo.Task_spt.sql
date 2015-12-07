USE [db]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Task_spt]') AND type in (N'P', N'PC'))
    exec('CREATE PROCEDURE [dbo].[Task_spt] AS BEGIN SET NOCOUNT ON; END')
GO

ALTER PROCEDURE [dbo].[Task_spt]
@Email varchar(50),
@Query varchar(50),
@Price int

AS
BEGIN
    SET NOCOUNT ON;
    
    IF NOT EXISTS (SELECT * FROM [dbo].[Emails] WHERE [Email] = @Email AND [Query] = @Query)
	BEGIN
	    INSERT INTO [dbo].[Emails] ([Email], [Query], [Price]) VALUES (@Email, @Query, @Price)
	END
END