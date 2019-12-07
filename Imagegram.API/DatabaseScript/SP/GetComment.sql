USE [Imagegram]
GO

/****** Object:  StoredProcedure [dbo].[WS_GetComment]    Script Date: 12/7/2019 8:23:50 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[WS_GetComment] 
	-- Add the parameters for the stored procedure here
	@PostID INT,
	@Limit INT = 20,
	@Page INT = 1
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT 
	CommentId AS 'CommentId'
	, Content AS 'Content'
	, PostId AS 'PostId'
	, CreatorUUID AS 'CreatorUUID'
	, CreatedDate AS 'CreatedDate'
	, TotalCount = Count(1) OVER()
	FROM
	[dbo].[Comment]
	WHERE PostId = @PostID
	ORDER BY CreatedDate DESC
	OFFSET @Limit * (@Page-1) ROWS
	FETCH NEXT @Limit ROWS ONLY;
END

GO

