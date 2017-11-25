CREATE PROCEDURE [dbo].[LongPrint]
      @String NVARCHAR(MAX)

AS

DECLARE
               @CurrentEnd BIGINT, /* track the length of the next substring */
               @offset tinyint /*tracks the amount of offset needed */

set @string = replace(  replace(@string, char(13) + char(10), char(10))   , char(13), char(10))

WHILE LEN(@String) > 1
BEGIN

IF CHARINDEX(CHAR(10), @String) between 1 AND 4000
    BEGIN

SET @CurrentEnd =  CHARINDEX(char(10), @String) -1
           set @offset = 2
    END
    ELSE
    BEGIN
           SET @CurrentEnd = 4000
            set @offset = 1
    END

PRINT SUBSTRING(@String, 1, @CurrentEnd)

set @string = SUBSTRING(@String, @CurrentEnd+@offset, 1073741822)

END /*End While loop*/