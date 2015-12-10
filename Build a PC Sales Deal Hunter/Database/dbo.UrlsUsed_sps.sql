USE [db]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UrlsUsed_sps]') AND type in (N'P', N'PC'))
    exec('CREATE PROCEDURE [dbo].[UrlsUsed_sps] AS BEGIN SET NOCOUNT ON; END')
GO

ALTER PROCEDURE [dbo].[UrlsUsed_sps]
@OriginalUrl varchar(200)

AS
BEGIN
    SET NOCOUNT ON;
	SELECT [ShortenedUrl] FROM [dbo].[UrlsUsed] WHERE [OriginalUrl] = @OriginalUrl
END