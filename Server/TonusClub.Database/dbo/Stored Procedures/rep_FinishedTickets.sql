

CREATE procedure rep_FinishedTickets(@days int, @companyid uniqueidentifier)
as
select d.Name Клуб, c.LastName as [Фамилия], c.FirstName [Имя], c.MiddleName [Отчество], c.Phone2 [Телефон], tt.Name [Абонемент],
	datediff(day, DATEADD(day,t.length, startdate), getdate()) - (ISNULL((select SUM(DATEDIFF(dd, StartDate, FinishDate)) from TicketFreezes tf where tf.TicketId = t.Id), 0)) [Дней с окончания]
from Tickets t
inner join customers c on c.Id=t.CustomerId
inner join TicketTypes tt on tt.Id=t.TicketTypeId
inner join Divisions d on d.Id=t.DivisionId
where not exists (select id from Tickets t1 where t1.CustomerId=t.CustomerId and t1.CreatedOn > t.CreatedOn)
and datediff(day, DATEADD(day,t.length, startdate), getdate()) > (@days - (ISNULL((select SUM(DATEDIFF(dd, StartDate, FinishDate)) from TicketFreezes tf where tf.TicketId = t.Id), 0)))
and datediff(day, DATEADD(day,t.length, startdate), getdate()) - (ISNULL((select SUM(DATEDIFF(dd, StartDate, FinishDate)) from TicketFreezes tf where tf.TicketId = t.Id), 0)) > @days
and (@companyid is null or @companyid = c.CompanyId)