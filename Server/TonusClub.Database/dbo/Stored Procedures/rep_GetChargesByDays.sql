
CREATE procedure [dbo].[rep_GetChargesByDays] (@start datetime, @end datetime, @companyId uniqueidentifier, @isauto bit)
as
select CAST(date as date) Дата, c.LastName Фамилия, c.FirstName Имя, c.MiddleName Отчество, c.Phone2 Телефон, d.Name Клуб, SUM(unitcharge) Списано, SUM(guestcharge) Гостевых 
from UnitCharges uc
inner join Tickets t on t.Id=TicketId
inner join Customers c on c.Id=t.CustomerId
inner join Divisions d on d.Id=t.DivisionId
where (@companyId is null or c.CompanyId=@companyId)
		and (@start is null or uc.Date >=@start)
		and (@end is null or uc.Date <= @end)
		and (@isauto is null or @isauto = 0 or uc.Reason = 'Автоматическое списание для смарт-абонементов')
group by CAST(date as date), c.LastName, c.FirstName, c.MiddleName, c.Phone2, d.Name
