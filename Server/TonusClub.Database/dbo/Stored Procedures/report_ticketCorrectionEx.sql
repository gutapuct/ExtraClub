
CREATE procedure [dbo].[report_ticketCorrectionEx](@start datetime, @end datetime, @divisionId uniqueidentifier, @number nvarchar(256), @companyId uniqueidentifier)
as
select crd.barcode as [Номер карты],
c.LastName [Фамилия], c.FirstName [Имя], c.MiddleName [Отчество],
 t.Number as [Номер абонемента],
co.createdOn as [Дата коррекции],
co.propertyname as [Свойство],
co.oldvalue as [Старое значение],
co.newvalue as [Новое значение],
co.comment as [Комментарий],
u.FullName [Сотрудник],
d.name Клуб
from TicketCorrections co
inner join Tickets t on t.Id=co.TicketId
inner join customers c on c.Id=t.CustomerId
cross apply (select top 1 ca.CardBarcode barcode from CustomerCards ca where ca.CustomerId=c.Id order by ca.EmitDate desc) as crd
inner join Users u on u.UserId=co.UserId
inner join Divisions d on d.Id=t.DivisionId
where 
(co.createdon>=@start or @start is null)
and (co.createdon<=@end or @end is null)
and (d.Id = @divisionId or @divisionId is null)
and (t.Number = @number or @number is null)
and (t.CompanyId=@companyId or @companyId is null)
