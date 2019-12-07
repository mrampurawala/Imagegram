USE [Imagegram]
GO

/****** Object:  StoredProcedure [dbo].[WS_DeleteAccount]    Script Date: 12/7/2019 8:22:57 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[WS_DeleteAccount]
	-- Add the parameters for the stored procedure here
	@UUID VARCHAR(MAX)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	IF EXISTS(SELECT UUID FROM Account WHERE UUID = @UUID)
	BEGIN
		
		DELETE FROM Comment WHERE CreatorUUID = @UUID
		DELETE FROM Post WHERE CreatorUUID = @UUID
		DELETE FROM Account WHERE UUID = @UUID

		SELECT 'Record Deleted'
	END
END

GO

