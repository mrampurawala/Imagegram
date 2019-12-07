USE [Imagegram]
GO

/****** Object:  StoredProcedure [dbo].[WS_CreatePost]    Script Date: 12/7/2019 8:22:46 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[WS_CreatePost]
	-- Add the parameters for the stored procedure here
	@ImageContent VARBINARY(MAX),
	@Comment VARCHAR(MAX),
	@UUID VARCHAR(MAX)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @PostId INT

	INSERT INTO Post(ImageContent,CreatedDate,CreatorUUID)
	VALUES
	(
		@ImageContent
		, GETDATE()
		, @UUID
	)

	SELECT @PostId = SCOPE_IDENTITY();
    -- Insert statements for procedure here
	INSERT INTO Comment(Content,PostId,CreatedDate,CreatorUUID) VALUES
	(
		@Comment
		, @PostId
		, GETDATE()
		, @UUID
	)
	
	SELECT @PostId
END

GO

