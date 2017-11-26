
CREATE PROCEDURE [dbo].rep_GetGoodFlow(@customer nvarchar(500), @good uniqueidentifier, @divisionId uniqueidentifier, @companyId uniqueidentifier)
as
set @customer = '%'+ISNULL(@customer, '')+'%';
select c.id _customerId, g.Name Товар, d.Name Клуб,c.LastName Фамилия, c.FirstName Имя, c.MiddleName Отчество, 
 sum(Amount) Зарезервировано, u.Name [ед.изм]
 from CustomerGoodsFlows gf
inner join Goods g on g.GoodId=gf.GoodId
inner join Customers c on c.Id=gf.CustomerId
inner join Divisions d on d.Id=gf.DivisionId
inner join UnitTypes u on u.Id=g.UnitTypeId
where isnull(LastName+ ' ','')+ISNULL(FirstName+' ','')+ISNULL(MiddleName,'') like @customer
and (@divisionId is null or gf.DivisionId=@divisionId)
and (@companyId is null or gf.CompanyId=@companyId)
and (@good is null or gf.GoodId=@good)
group by c.Id, g.Name, d.Name, c.FirstName, c.MiddleName, c.LastName, u.Name

