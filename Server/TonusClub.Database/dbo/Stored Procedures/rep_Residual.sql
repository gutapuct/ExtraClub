CREATE procedure [dbo].[rep_Residual](@companyId uniqueidentifier)
as
declare @left table(TicketId uniqueidentifier, UnitsLeft int)
insert into @left
select t.Id, t.UnitsAmount-isnull(sum(unitcharge),0)
from tickets t
left join UnitCharges ch on ch.TicketId=t.Id
where (t.CompanyId=@companyId or @companyId is null) and returndate is null
group by t.Id, t.UnitsAmount

declare @loan table (TicketId uniqueidentifier, Loan money)
insert into @loan
select t.Id,  Price * (1 - DiscountPercent)-isnull(sum(amount),0)
from tickets t
left join TicketPayments p on p.TicketId=t.Id
where (t.CompanyId=@companyId or @companyId is null) and returndate is null
group by t.Id, t.Price, t.DiscountPercent

declare @lens table(TicketId uniqueidentifier, Length int)
insert into @lens
select t.Id, t.Length+isnull(sum(DATEDIFF(DAY, tf.StartDate, tf.FinishDate)),0)
from tickets t
left join TicketFreezes tf on tf.TicketId=t.Id and (DATEDIFF(DAY, tf.StartDate, FinishDate)<1000 or tf.StartDate is null)
where (t.CompanyId=@companyId or @companyId is null) and returndate is null
group by t.Id, t.Length

declare @result table(CompanyId uniqueidentifier, TicketId uniqueidentifier, Perc money, Cost money, unitsamount int, residual money)
insert into @result
select t.CompanyId, t.Id, isnull((l.Length-cast(DATEDIFF(DAY, t.StartDate, GETDATE()) as money))/l.Length,1), Price * (1 - DiscountPercent), UnitsAmount, null
from tickets t
left join @lens l on l.TicketId=t.Id
where (t.CompanyId=@companyId or @companyId is null) and UnitsAmount>0 and t.Length>0 
and isnull((l.Length-cast(DATEDIFF(DAY, t.StartDate, GETDATE()) as money))/l.Length,1)>0 and returndate is null

update p
set residual = (Cost / UnitsAmount * isnull(UnitsLeft,0) * ResidualValueK11 * ResidualValueK2 - ResidualValueS1 - lo.Loan )* (1 - TicketReturnPercentCommission) - TicketReturnFixedCommission
from @result p
inner join Companies c on c.CompanyId=p.CompanyId
left join @left l on l.TicketId=p.TicketId
left join @loan lo on lo.TicketId=p.TicketId
where p.Perc >= ResidualValueP1

update p
set residual = (Cost / UnitsAmount * isnull(UnitsLeft,0) * ResidualValueK12 * ResidualValueK2 - ResidualValueS1 - lo.Loan)* (1 - TicketReturnPercentCommission) - TicketReturnFixedCommission
from @result p
inner join Companies c on c.CompanyId=p.CompanyId
left join @left l on l.TicketId=p.TicketId
left join @loan lo on lo.TicketId=p.TicketId
where p.Perc >= ResidualValueP2 and residual is null

update p
set residual = (Cost / UnitsAmount * isnull(UnitsLeft,0) * ResidualValueK13 * ResidualValueK2 - ResidualValueS1 - lo.Loan)* (1 - TicketReturnPercentCommission) - TicketReturnFixedCommission
from @result p
inner join Companies c on c.CompanyId=p.CompanyId
left join @left l on l.TicketId=p.TicketId
left join @loan lo on lo.TicketId=p.TicketId
where residual is null


select c.LastName Фамилия, c.FirstName Имя, c.MiddleName Отчество, c.Phone2 Телефон,
d.Name Клуб, tt.Name Название, t.Number Номер, t.CreatedOn Продан, r.Cost Стоимость,
t.unitsamount [Всего ед.], t.UnitsAmount-l.UnitsLeft Потрачено, l.UnitsLeft Осталось, t.Length Длительность, lens.Length - DATEDIFF(day, startdate, getdate()) [Дней осталось],
r.Cost-lo.Loan Оплачено,lo.Loan Долг,r.residual [К возврату]
from @result r
inner join Tickets t on t.Id=r.TicketId
inner join customers c on c.Id=t.CustomerId
inner join Divisions d on d.Id=t.DivisionId
inner join TicketTypes tt on tt.Id=t.TicketTypeId
left join @left l on l.TicketId=t.Id
left join @lens lens on lens.TicketId=t.Id
left join @loan lo on lo.TicketId=t.Id

