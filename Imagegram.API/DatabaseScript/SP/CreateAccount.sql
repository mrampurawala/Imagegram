USE [Imagegram]
GO

/****** Object:  StoredProcedure [dbo].[WS_CreateAccount]    Script Date: 12/7/2019 8:22:25 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[WS_CreateAccount]
	-- Add the parameters for the stored procedure here
	@Name VARCHAR(MAX),
	@UUID VARCHAR(MAX)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	INSERT INTO Account(NAME,UUID,CreatedDate,Active) VALUES
	(
		@Name,
		@UUID,
		GETDATE(),
		1
	)
	SELECT @UUID
END

GO

