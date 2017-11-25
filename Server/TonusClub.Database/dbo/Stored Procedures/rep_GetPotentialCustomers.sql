


CREATE procedure [dbo].[rep_GetPotentialCustomers] (@companyId uniqueidentifier) as
begin

declare @tmp table (CustomerId uniqueidentifier, EventDate datetime, Result nvarchar(MAX));

WITH summary AS (
	SELECT p.CustomerId, p.EventDate, p.Result, ROW_NUMBER() OVER(PARTITION BY p.CustomerId ORDER BY p.EventDate DESC) AS rowNumber
    FROM (
		select customerid, eventdate, Result
		from CustomerCrmEvents
		union
		select CustomerId, isnull(completedon,expirydate), cn.CompletionComment
		from CustomerNotifications cn
		where cn.CompletedOn is not null
	)
as p )

insert into @tmp
SELECT s.CustomerId, EventDate, Result
FROM summary s
WHERE s.rowNumber = 1

declare @tmp2 table (CustomerId uniqueidentifier, expirydate datetime, Message nvarchar(MAX));

WITH summary2 AS (
	SELECT p.CustomerId, p.expirydate, p.Message, ROW_NUMBER() OVER(PARTITION BY p.CustomerId ORDER BY p.expirydate) AS rowNumber
    FROM (
		select CustomerId, expirydate, cn.Message
		from CustomerNotifications cn
		where cn.CompletedOn is null
	)
as p )

insert into @tmp2
SELECT s.CustomerId, expirydate, Message
FROM summary2 s
WHERE s.rowNumber = 1

declare @tmpConsultations table (CustomerId uniqueidentifier, CreatedOn datetime, VisitDate datetime);

WITH summary3 AS (
	SELECT p.CustomerId, p.CreatedOn, p.VisitDate, ROW_NUMBER() OVER(PARTITION BY p.CustomerId ORDER BY p.CreatedOn DESC) AS rowNumber
    FROM (
		select te.CustomerId, te.CreatedOn, te.VisitDate
		from TreatmentEvents te
		inner join TreatmentConfig tc on tc.Id = te.TreatmentConfigId
		where te.VisitStatus <> 1
		and tc.TreatmentTypeId = '73EE31C6-379E-49C3-8ADE-EB1F46282523'
	)
as p)

insert into @tmpConsultations
SELECT s.CustomerId, s.CreatedOn, s.VisitDate
FROM summary3 s
WHERE s.rowNumber = 1

declare @tmpTickets table (CustomerId uniqueidentifier, CreatedOn datetime, Price money, Units int);

WITH summary4 AS (
	SELECT p.CustomerId, p.CreatedOn, p.Price, p.Units, ROW_NUMBER() OVER(PARTITION BY p.CustomerId ORDER BY p.CreatedOn DESC) AS rowNumber
    FROM (
		select t.CustomerId, t.CreatedOn, t.Price * (1 - t.DiscountPercent) [Price], tt.Units / 8 [Units]
		from tickets t
		inner join tickettypes tt on tt.id = t.TicketTypeId
		where t.ReturnDate is null and tt.IsGuest = 0 and tt.IsVisit = 0 and tt.IsSmart = 1
	)
as p)

insert into @tmpTickets
SELECT s.CustomerId, s.CreatedOn, s.Price, s.Units
FROM summary4 s
WHERE s.rowNumber = 1

select c.id _customerId, c.CreatedOn [Дата создания], LastName Фамилия,
 FirstName Имя, MiddleName Отчество, div.Name Клуб, Phone2 Мобильный, ag.Name [Рекламная группа], at.name [Рекламный канал],
 AdvertComment Подробнее,
 t3.CreatedOn [Дата записи на консультацию], t3.VisitDate [Дата проведения консультации],
 STUFF((    SELECT ',' + st.name AS [text()]
                        FROM dbo.CustomersCustomerStatuses cst
                        inner join dbo.CustomerStatuses st on st.Id=cst.CustomerStatusId
                        WHERE
                        cst.CustomerId=c.id
                        FOR XML PATH('')
                        ), 1, 1, '' ) Статус,
 t4.CreatedOn [Дата покупки первого абонемента], t4.Price [Сумма первого купленного абонемента], t4.Units [Количество тренировок в первом абонементе],
 t.EventDate [Дата последнего события], t.Result [Результат последнего события],
 t2.expirydate [Дата запланированного звонка], t2.Message [Тема запланированного звонка]
from customers c
left join AdvertTypes at on at.Id = c.AdvertTypeId
left join AdvertGroups ag on ag.Id = at.AdvertGroupId
left join @tmp t on t.CustomerId=c.Id
left join @tmp2 t2 on t2.CustomerId=c.Id
left join @tmpConsultations t3 on t3.CustomerId=c.Id
left join @tmpTickets t4 on t4.CustomerId=c.Id
inner join Divisions div on div.id = c.ClubId
where IsEmployee = 0
and not exists (select id from Employees em where em.BoundCustomerId=c.Id)
and (c.companyid=@companyId or @companyid is null)
order by 2 desc,3,4
end
