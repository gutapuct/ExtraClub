create procedure sp_UpdateCrmSales (@divId uniqueidentifier, @amount money)
as
declare @clubid uniqueidentifier
declare @valid uniqueidentifier

select @clubid=Id from crm..clubs where DivisionId=@divId and deletedon is null
if @clubid is not null
begin
	select @valid=id from crm..metavalues where MetaFieldId='5a433787-4007-45c8-9728-20931624b324' and EntityId=@clubid
	if @valid is not null
	begin
		update crm..metavalues set NumberValue=@amount where id=@valid
	end
	else
	begin
		insert into crm..metavalues
		SELECT NEWID(),'5a433787-4007-45c8-9728-20931624b324', @clubid, null,@amount,null,null,null
	end
end