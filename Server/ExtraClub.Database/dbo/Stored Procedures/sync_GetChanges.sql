CREATE procedure [dbo].[sync_GetChanges](@company uniqueidentifier, @version bigint)
as

declare @sql varchar(max)
set @sql = ''
SELECT @sql=@sql+
'
SELECT '''+object_name(tt.object_id)+''', c.'+replace(kcs.keycol,',',',c.')+', SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES '+object_name(tt.object_id)+', '+cast(@version as nvarchar(50))+') AS c
LEFT OUTER JOIN '+object_name(tt.object_id)+' AS e ON '+dbo.splitdouble(kcs.keycol)+' '+
case(ca.name) when 'CompanyId' then '
where e.companyId='''+convert(nvarchar(36), @company)+''' or e.companyId is null' else ' ' end+
'
'
from sys.change_tracking_tables tt
outer apply (select * from syscolumns sci where sci.id=tt.object_id and sci.name='CompanyId') as ca

cross apply (
select substring((
select ','+c.name
from sys.objects pks
inner join sys.indexes i on i.name=pks.name
inner join sys.index_columns ic on ic.object_id=i.object_id and ic.index_id=i.index_id
inner join sys.columns c on c.object_id=ic.object_id and c.column_id=ic.column_id
where pks.type='PK'
and pks.parent_object_id=tt.object_id
FOR XML PATH('')),2,999) keycol) as kcs
where ca.name is not null or kcs.keycol like '%,%'
set @sql = @sql+'Select null,null where 1=2'
--print @sql
exec (@sql)
GO