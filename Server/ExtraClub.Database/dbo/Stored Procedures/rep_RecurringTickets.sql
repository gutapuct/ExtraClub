
CREATE procedure rep_RecurringTickets(@companyId uniqueidentifier)
as
select d.Name [Клуб], c.LastName Фамилия, c.FirstName Имя, c.MiddleName Отчество, c.Phone2 [Телефон],
	ttNew.Name [Новый абонемент], tNew.Number [Номер нового], tnew.Price*(1-isnull(tnew.DiscountPercent,0)) [Стоимость], tnew.CreatedOn [Дата нового],
	tOld.Name [Старый абонемент], tOld.Number [Номер старого], tOld.CreatedOn [Дата старого]
from tickets tNew
inner join TicketTypes ttNew on ttNew.Id=tNew.TicketTypeId
inner join Customers c on c.Id = tNew.CustomerId
inner join Divisions d on d.Id= tNew.DivisionId
cross apply (
	select top 1 ttOld.Name, tOld.CreatedOn, Number from Tickets tOld
	inner join TicketTypes ttOld on ttOld.Id=tOld.TicketTypeId
	where tOld.CustomerId=tNew.CustomerId
			and tOld.CreatedOn < tNew.CreatedOn
			and ttOld.IsGuest=0 and ttOld.IsVisit=0
			and (tOld.InheritedTicketId is null or tOld.InheritedTicketId<>tNew.Id)
	order by tOld.CreatedOn desc
	) as tOld
where ttNew.IsGuest=0 and ttNew.IsVisit=0
		and (@companyId is null or @companyId=c.CompanyId)

