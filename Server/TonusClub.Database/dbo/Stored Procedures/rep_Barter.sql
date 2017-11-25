CREATE procedure [dbo].[rep_Barter] (@companyId uniqueidentifier, @start datetime, @end datetime, @divisionId uniqueidentifier)
as
select cast(datepart(month, t.createdon) as nvarchar(2))+'/'+cast(datepart(year, t.createdon) as nvarchar(4)) Месяц,
t.Price Цена,
t.UnitsAmount Единиц,
substring(tt.visitstart,1,2)+':'+substring(tt.visitstart,3,2)+' - '+substring(tt.visitend,1,2)+':'+substring(tt.visitend,3,2) Время,
t.number Номер,
cast(t.createdon as date) Оформлен,
d.name Клуб,
c.LastName+' '+c.firstname+isnull(' '+c.middlename,'') Клиент,
c.WorkPlace Компания,
u.fullname [Кем выдан],
startdate [Активирован],
dateadd(day, t.length, isnull(startdate, dateadd(day, tt.autoactivate,t.createdon)))[Истекает],
tt.name Тип
from tickets t
inner join tickettypes tt on tt.id=t.tickettypeid
inner join divisions d on d.id=t.divisionid
inner join customers c on c.id=t.customerid
inner join users u on u.userid=t.createdby
where t.discountpercent=1
and (t.companyid=@companyId or @companyid is null)
and t.createdon>=@start and dateadd(day, 1 , cast(t.createdon as date))<@end
and (t.divisionid=@divisionid or @divisionid is null)
