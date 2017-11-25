EXEC sp_msforeachtable "begin try ALTER TABLE ? ENABLE CHANGE_TRACKING WITH(TRACK_COLUMNS_UPDATED = OFF) end try begin catch end catch"
GO
ALTER TABLE [dbo].LocalSettings DISABLE CHANGE_TRACKING 
GO
ALTER TABLE [dbo].[DictionaryInfo] DISABLE CHANGE_TRACKING 
GO
ALTER TABLE [dbo].[ReportInfos] DISABLE CHANGE_TRACKING 
GO
ALTER TABLE [dbo].[ReportParameters] DISABLE CHANGE_TRACKING 
GO
ALTER TABLE [dbo].[Log] DISABLE CHANGE_TRACKING 
GO

CREATE FUNCTION [dbo].[Split]
(    
    @RowData NVARCHAR(MAX),
    @Delimeter NVARCHAR(MAX)
)
RETURNS @RtnValue TABLE 
(
    ID INT IDENTITY(1,1),
    Data NVARCHAR(MAX)
) 
AS
BEGIN 
    DECLARE @Iterator INT
    SET @Iterator = 1

    DECLARE @FoundIndex INT
    SET @FoundIndex = CHARINDEX(@Delimeter,@RowData)

    WHILE (@FoundIndex>0)
    BEGIN
        INSERT INTO @RtnValue (data)
        SELECT 
            Data = LTRIM(RTRIM(SUBSTRING(@RowData, 1, @FoundIndex - 1)))

        SET @RowData = SUBSTRING(@RowData,
                @FoundIndex + DATALENGTH(@Delimeter) / 2,
                LEN(@RowData))

        SET @Iterator = @Iterator + 1
        SET @FoundIndex = CHARINDEX(@Delimeter, @RowData)
    END
    
    INSERT INTO @RtnValue (Data)
    SELECT Data = LTRIM(RTRIM(@RowData))

    RETURN
END

GO

create function dbo.SplitDouble (@str nvarchar(max))
returns nvarchar(max)
as
begin
declare @res nvarchar(max)
set @res=''
select @res = @res+'e.'+data+'=c.'+data+' and ' from dbo.split(@str, ',')

return substring(@res,1,len(@res)-4)
end

GO

--CREATE procedure [dbo].[sync_GetChanges](@company uniqueidentifier, @version bigint)
--as

--declare @sql varchar(max)
--set @sql = ''
--SELECT @sql=@sql+
--'
--SELECT '''+object_name(tt.object_id)+''', c.'+replace(kcs.keycol,',',',c.')+', SYS_CHANGE_OPERATION ChType
--FROM CHANGETABLE (CHANGES '+object_name(tt.object_id)+', '+cast(@version as nvarchar(50))+') AS c
--LEFT OUTER JOIN '+object_name(tt.object_id)+' AS e ON '+dbo.splitdouble(kcs.keycol)+' '+
--case(ca.name) when 'CompanyId' then '
--where e.companyId='''+convert(nvarchar(36), @company)+''' or e.companyId is null' else ' ' end+
--'
--'
--from sys.change_tracking_tables tt
--outer apply (select * from syscolumns sci where sci.id=tt.object_id and sci.name='CompanyId') as ca

--cross apply (
--select substring((
--select ','+c.name
--from sys.objects pks
--inner join sys.indexes i on i.name=pks.name
--inner join sys.index_columns ic on ic.object_id=i.object_id and ic.index_id=i.index_id
--inner join sys.columns c on c.object_id=ic.object_id and c.column_id=ic.column_id
--where pks.type='PK'
--and pks.parent_object_id=tt.object_id
--FOR XML PATH('')),2,999) keycol) as kcs
--set @sql = @sql+'Select null,null where 1=2'
----print @sql
--exec (@sql)
--GO


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

GO

---REGIONAL:
alter procedure [dbo].[sync_GetChanges](@company uniqueidentifier, @version bigint)
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