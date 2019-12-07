USE [Imagegram]
GO

/****** Object:  StoredProcedure [dbo].[WS_CreateComment]    Script Date: 12/7/2019 8:22:37 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[WS_CreateComment]
	-- Add the parameters for the stored procedure here
	@PostID INT,
	@Comment VARCHAR(MAX),
	@UUID VARCHAR(MAX)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
    -- Insert statements for procedure here
	IF EXISTS(SELECT PostId FROM POST WHERE PostId = @PostID)
	BEGIN
		INSERT INTO Comment(Content,PostId,CreatedDate,CreatorUUID) VALUES
		(
			@Comment
			, @PostId
			, GETDATE()
			, @UUID
		)
		SELECT SCOPE_IDENTITY()
	END
END

GO

