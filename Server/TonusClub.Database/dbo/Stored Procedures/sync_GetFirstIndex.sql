
CREATE procedure [dbo].[sync_GetFirstIndex](@companyId uniqueidentifier)
as

declare @sql varchar(max)
set @sql = ''
SELECT @sql=@sql+
'
SELECT '''+object_name(tt.object_id)+''', e.'+replace(kcs.keycol,',',',e.')+'
FROM '+object_name(tt.object_id)+' AS e
CROSS APPLY CHANGETABLE (VERSION '+object_name(tt.object_id)+', ('+kcs.keycol+'), (e.'+replace(kcs.keycol,',',',e.')+')) as c '+
case(ca.name) when 'CompanyId' then 'where e.companyId='''+convert(nvarchar(36), @companyId)+'''' else ' ' end+
'  '
from sys.change_tracking_tables tt
--inner join syscolumns sc on sc.id=tt.object_id

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
set @sql = @sql+'Select null,null where 1=2'
exec (@sql)

