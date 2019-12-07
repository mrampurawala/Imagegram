USE [Imagegram]
GO

/****** Object:  StoredProcedure [dbo].[WS_DeleteComment]    Script Date: 12/7/2019 8:23:21 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[WS_DeleteComment]
	-- Add the parameters for the stored procedure here
	@PostID INT,
	@CommentID INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	IF EXISTS(SELECT CommentId FROM Comment WHERE CommentId = @CommentID AND PostId = @PostID)
	BEGIN
		DELETE FROM Comment WHERE CommentId = @CommentID AND PostId = @PostID
		SELECT 'Record Deleted'
	END
END

GO

