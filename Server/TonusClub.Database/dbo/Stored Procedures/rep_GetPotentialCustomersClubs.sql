
CREATE procedure [dbo].[rep_GetPotentialCustomersClubs] (@companyId uniqueidentifier) as
begin

declare @tmp table (CustomerId uniqueidentifier, EventDate datetime, Subject nvarchar(4000));
WITH summary AS (
SELECT p.CustomerId, p.EventDate, p.Subject, ROW_NUMBER() OVER(PARTITION BY p.CustomerId ORDER BY p.EventDate DESC) AS rowNumber
      FROM (select customerid, eventdate, subject from CustomerCrmEvents union select CustomerId, isnull(completedon,expirydate), Message from CustomerNotifications) p)
insert into @tmp
SELECT s.CustomerId, EventDate, subject
  FROM summary s
 WHERE s.rowNumber = 1


SELECT c.id _customerId, firstname Имя, middlename Отчество,lastname Фамилия, phone2 Мобильный, isnull(d2.name, isnull(d1.name, isnull(d.name, d3.name))) Клуб,
 t.EventDate [Дата события], t.Subject [Тема события], c.CreatedOn [Дата внесения],
 STUFF(( SELECT ', ' + st.name AS [text()]
		FROM dbo.CustomersCustomerStatuses cst
		inner join dbo.CustomerStatuses st on st.Id=cst.CustomerStatusId
		WHERE
		cst.CustomerId=c.id
		FOR XML PATH('')
		), 1, 1, '' ) Статус,
		at.name [Откуда узнал],
		AdvertComment Подробнее

  FROM Customers c
  left join AdvertTypes at on at.Id = c.AdvertTypeId
  left join calls ca on ca.customerid=c.id
  left join divisions d on d.id=ca.divisionid
  left join divisions d1 on d1.id=c.clubid
  outer apply (select top 1 t1.divisionid id from tickets t1 where t1.customerid=c.id) ti
    left join divisions d2 on d2.id=c.clubid
    
  inner join users u on u.userid=c.authorid
  left join employees e on e.id=u.employeeid
  left join divisions d3 on d3.id=e.maindivisionid  
  left join employees e1 on e1.boundcustomerid=c.id
  left join @tmp t on t.CustomerId=c.Id
  where not exists (
  select t.id
  from tickets t
  where t.customerid=c.id and t.tickettypeid not in (
  '6B4BEB26-1639-4B3A-AD72-E466EB8005A9',--ДОД
  'D377417C-3B9B-4089-ABB5-127A84EB2E84'--Гостевой
  ,'DAE1A3FB-1317-4307-9DBE-1442E7F7B920')--Еще дод
  ) and len(c.phone2)>2
  and e1.id is null
and (c.companyid=@companyId or @companyid is null)

order by 2,3,4
end

