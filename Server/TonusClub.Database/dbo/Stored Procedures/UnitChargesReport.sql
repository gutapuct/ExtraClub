
CREATE procedure [dbo].[UnitChargesReport] (@number nvarchar(30), @companyId uniqueidentifier)
as
select uc.Date as Дата, uc.Reason Основание, uc.UnitCharge Количество, v.InTime Пришел, v.OutTime Ушел from UnitCharges uc
inner join Tickets t on t.Id=uc.TicketId
outer apply (select top 1 * from CustomerVisits cv where cv.customerid=t.customerid and DATEPART(YEAR, cv.InTime)=DATEPART(YEAR, uc.Date)
 and DATEPART(MONTH, cv.InTime)=DATEPART(month, uc.Date)
  and DATEPART(day, cv.InTime)=DATEPART(DAY, uc.Date)) as v
where t.number = @number
and (t.companyid=@companyId or @companyid is null)
