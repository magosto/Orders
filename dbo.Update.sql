CREATE PROCEDURE [dbo].[Update]
	@number int,
	@paid bit
AS
BEGIN 
	UPDATE Orders Set [paid] = @paid
	WHERE number = @number
END
