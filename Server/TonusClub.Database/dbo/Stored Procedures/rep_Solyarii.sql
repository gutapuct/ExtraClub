
CREATE procedure [dbo].[rep_Solyarii](@start DateTime, @end DateTime, @companyid uniqueidentifier, @divisionid uniqueidentifier) as
begin

select isnull(c.LastName + ' ','') + isnull(c.FirstName + ' ','')+isnull(c.MiddleName, '') ФИО, D.Name [Клуб],
CONVERT (varchar, SV.VisitDate, 102) as [Дата визита], CONVERT(varchar, SV.VisitDate, 108) [Время визита],
S.Name [Название], SV.Amount [Кол-во минут], isnull(T.Number,'') [Списано с абонемента], isnull(cast(Cost as nvarchar),'')[Оплата нал],
case isnull(Status,'')
	when'2'
		then 'Завершена'
	when'1'
		then'Отменена'
	when'3'
		then'Прогуляна'
	when'0'
		then'Запланирована'
end [Статус]

from Customers C
inner join SolariumVisits SV on SV.CustomerId=C.Id
inner join Divisions D on sv.DivisionId=d.Id
inner join Solariums S on S.Id=SV.SolariumId
left join Tickets T on T.Id=Sv.TicketId
left join TicketTypes TT on TT.Id=T.TicketTypeId

where ((SV.VisitDate > @start) and (SV.VisitDate < dateadd(day, 1, @end)))
and (D.companyid=@companyid or @companyid is null)
and (D.id = @divisionid or @divisionid is null)

order by [Дата визита] desc, [Время визита] desc

end

