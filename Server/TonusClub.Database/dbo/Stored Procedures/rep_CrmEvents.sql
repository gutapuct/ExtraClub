

CREATE procedure [dbo].[rep_CrmEvents] (@start DateTime, 
@end DateTime, @companyid uniqueidentifier, @divisionid uniqueidentifier)as
	begin
set @end=dateadd(day, 1, @end)
select c.id _customerId, 
isnull(c.lastname + ' ', '') + isnull(c.FirstName +  ' ', '') + isnull(c.MiddleName, '') ФИО,
c.Phone2 Телефон,
ce.CreatedOn [Дата события],
ce.Subject [Тип/Тема],
u.FullName Сотрудник,
ce.Result Результат,
ce.Comment Комментарий,
STUFF(( SELECT ', ' + st.name
		FROM dbo.CustomersCustomerStatuses cst
		inner join dbo.CustomerStatuses st on st.Id=cst.CustomerStatusId
		WHERE
		cst.CustomerId=c.id
		FOR XML PATH('')
		), 1, 1, '' ) Статус,
at.Name [Рекламный канал]
from customercrmevents ce
inner join Customers c on c.Id=ce.CustomerId
left join AdvertTypes at on at.Id=c.AdvertTypeId
inner join Users u on u.UserId=ce.CreatedBy
inner join Divisions d on d.Id=ce.DivisionId
where ce.createdon >= @start and ce.createdon<= @end
and (@companyid is null or ce.CompanyId=@companyid)
and (@divisionid is null or ce.DivisionId=@divisionid)
union
select c.id _customerId, 
isnull(c.lastname + ' ', '') + isnull(c.FirstName +  ' ', '') + isnull(c.MiddleName, '') ФИО,
c.phone2,
cl.StartAt,
case cl.IsIncoming when 1 then 'Входящий звонок' else 'Исходящий звонок' end,
u.FullName,
cl.Result,
cl.Log,
STUFF(( SELECT ', ' + st.name
		FROM dbo.CustomersCustomerStatuses cst
		inner join dbo.CustomerStatuses st on st.Id=cst.CustomerStatusId
		WHERE
		cst.CustomerId=c.id
		FOR XML PATH('')
		), 1, 1, '' ) Статус,
at.Name
from Calls cl
inner join Customers c on c.Id=cl.CustomerId
left join AdvertTypes at on at.Id=c.AdvertTypeId
inner join Users u on u.UserId=cl.CreatedBy
inner join Divisions d on d.Id=cl.DivisionId
where cl.StartAt >= @start and cl.startat<= @end
and (@companyid is null or cl.CompanyId=@companyid)
and (@divisionid is null or cl.DivisionId=@divisionid)
order by 3

end

