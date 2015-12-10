USE [db]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UrlsUsed_spt]') AND type in (N'P', N'PC'))
    exec('CREATE PROCEDURE [dbo].[UrlsUsed_spt] AS BEGIN SET NOCOUNT ON; END')
GO

ALTER PROCEDURE [dbo].[UrlsUsed_spt]
@OriginalUrl varchar(200),
@ShortenedUrl varchar(200)

AS
BEGIN
    SET NOCOUNT ON;
	    IF NOT EXISTS (SELECT * FROM [dbo].[UrlsUsed] WHERE [OriginalUrl] = @OriginalUrl)
	BEGIN
		INSERT INTO [dbo].[UrlsUsed] ([OriginalUrl], [ShortenedUrl]) VALUES (@OriginalUrl, @ShortenedUrl)
	END
END