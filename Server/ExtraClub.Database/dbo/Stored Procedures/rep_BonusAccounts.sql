Create procedure rep_BonusAccounts (@start DateTime, @end DateTime, @companyid uniqueidentifier) as
begin
select c.id _customerId,
	isnull(c.LastName+' ', '')+ isnull(c.firstname+ ' ', '') + isnull(c.middlename,'') [Клиент],
	ba.createdon [Дата], ba.amount [Сумма], ba.Description [Описание], u.fullname [Кто провел]
from BonusAccounts ba
inner join Customers c on c.id = ba.customerid
inner join users u on u.userid = ba.createdby

where ((ba.createdon > @start) and (ba.createdon < dateadd(day, 1, @end)))
and (c.companyid=@companyid or @companyid is null)
order by ba.createdon desc
end
