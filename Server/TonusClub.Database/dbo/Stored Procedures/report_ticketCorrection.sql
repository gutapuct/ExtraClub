
CREATE procedure [dbo].[report_ticketCorrection](@start datetime, @end datetime, @divisionId uniqueidentifier, @companyId uniqueidentifier)
as
select crd.barcode as [Номер карты],
c.LastName [Фамилия], c.FirstName [Имя], c.MiddleName [Отчество],
 t.Number as [Номер абонемента],
tp.PaymentDate as [Дата оформления],
firstpayment.PaymentDate [Заднее число],
firstpayment.Amount [Сумма задним числом],
u.FullName [Сотрудник],
d.name Клуб
from TicketPayments tp
inner join Tickets t on t.Id=tp.TicketId
inner join customers c on c.Id=t.CustomerId
cross apply (select top 1 ca.CardBarcode barcode from CustomerCards ca where ca.CustomerId=c.Id order by ca.EmitDate desc) as crd
cross apply (select top 1 tp1.amount, tp1.paymentdate from TicketPayments tp1 where tp1.TicketId=t.ID and Amount>0 order by PaymentDate) as firstpayment
inner join Users u on u.UserId=t.CreatedBy
inner join Divisions d on d.Id=t.DivisionId
where tp.Amount=0
and t.StartDate>=@start and t.StartDate<=@end
and (d.Id = @divisionId or @divisionId is null)
and (t.CompanyId=@companyId or @companyId is null)

