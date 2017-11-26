
create function dbo.SplitDouble (@str nvarchar(max))
returns nvarchar(max)
as
begin
declare @res nvarchar(max)
set @res=''
select @res = @res+'e.'+data+'=c.'+data+' and ' from dbo.split(@str, ',')

return substring(@res,1,len(@res)-4)
end

