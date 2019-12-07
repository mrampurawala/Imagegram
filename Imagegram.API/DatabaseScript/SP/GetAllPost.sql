USE [Imagegram]
GO

/****** Object:  StoredProcedure [dbo].[WS_GetAllPost]    Script Date: 12/7/2019 8:23:31 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[WS_GetAllPost] 
	-- Add the parameters for the stored procedure here
	@Limit INT = 20,
	@Page INT = 1
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT  p.PostId, p.ImageContent, p.CreatedDate,p.CreatorUUID , com.CommentId
	, com.Content, com.CreatedDate,com.CreatorUUID
	--, TotalCount = Count(1) OVER()
	FROM    Post p
	CROSS APPLY
			(
				SELECT  TOP 3 c.CommentId, c.Content,c.CreatedDate,c.CreatorUUID
				FROM    Comment c
				WHERE   c.PostId = p.PostId
				ORDER BY c.CreatedDate DESC
			) com
	--ORDER BY p.CreatedDate DESC
	--OFFSET @Limit * (@Page-1) ROWS
	--FETCH NEXT @Limit ROWS ONLY;
END




GO

