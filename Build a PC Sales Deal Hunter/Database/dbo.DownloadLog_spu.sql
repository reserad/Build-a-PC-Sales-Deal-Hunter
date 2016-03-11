USE [db]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DownloadLog_spu]') AND type in (N'P', N'PC'))
    exec('CREATE PROCEDURE [dbo].[DownloadLog_spu] AS BEGIN SET NOCOUNT ON; END')
GO

ALTER PROCEDURE [dbo].[DownloadLog_spu]

AS
BEGIN
    SET NOCOUNT ON;
    UPDATE [dbo].[DownloadLog] SET NumberOfDownloads = NumberOfDownloads + 1;
END