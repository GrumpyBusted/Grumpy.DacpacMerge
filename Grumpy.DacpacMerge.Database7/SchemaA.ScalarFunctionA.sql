﻿CREATE FUNCTION [SchemaA].[ScalarFunctionA]
(
	@param1 int,
	@param2 int
)
RETURNS INT
AS
BEGIN
	RETURN @param1 + @param2
END
