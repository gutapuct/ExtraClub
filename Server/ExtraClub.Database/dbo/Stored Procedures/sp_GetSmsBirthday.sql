
CREATE procedure [dbo].[sp_GetSmsBirthday]
as
select c.Id CustomerId, co.UtcCorr, c.Phone2, c.Email, c.SmsList, mc.SendSms, mc.SmsBirthday
from customers c
inner join companies co on co.CompanyId=c.CompanyId
inner join syncmetadata..metacompanies mc on mc.DivisionId=c.ClubId
where DATEPART(day, c.Birthday)=DATEPART(day, getdate())
and DATEPART(month, c.Birthday)=DATEPART(month, getdate())
and not exists (select * from BonusAccounts ba where ba.CustomerId=c.Id and ba.Description = 'Начисление бонусов в честь Дня рождения клиента' and datepart(year, ba.CreatedOn)=DATEPART(YEAR, GETDATE()))

