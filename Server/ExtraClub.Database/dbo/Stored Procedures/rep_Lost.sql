
CREATE procedure [dbo].[rep_Lost] (@fromN int, @toN int, @companyid uniqueidentifier, @divisionid uniqueidentifier) as
begin
select c.id _customerId, isnull(c.lastname + ' ', '') + isnull(c.FirstName +  ' ', '') + isnull(c.MiddleName + ' ', '') as [ФИО],
	IsNull((select cc.CardBarcode from CustomerCards cc where cc.CustomerId = c.id and cc.IsActive = 1), '') [Номер карты],
	c.Phone2 Телефон, d.name Клуб,
	STUFF(( SELECT ', ' + st.name AS [text()]
		FROM dbo.CustomersCustomerStatuses cst
		inner join dbo.CustomerStatuses st on st.Id=cst.CustomerStatusId
		WHERE cst.CustomerId=c.id
		FOR XML PATH('')
		), 1, 1, '' ) Статус,
	CAST(cv.InTime as DATE) as [Дата последнего посещения],
	CAST(GETDATE() - CAST(cv.InTime as DATE) as int) [Число дней с последнего посещения],
	(select top(1) ISNULL(cn.CompletedOn, cn.ExpiryDate) from CustomerNotifications cn where cn.CustomerId = c.id order by ISNULL(cn.CompletedOn, cn.ExpiryDate)  desc) [Дата последней задачи]
	
from Customers c
cross apply (select top 1 id from CustomerVisits cv
                where cv.customerid = c.id
				order by cv.InTime desc
            ) ca
inner join CustomerVisits cv on (cv.CustomerId = c.id and ca.id = cv.id)
inner join Divisions d on c.ClubId = d.id
where (CAST(GETDATE() - CAST(cv.InTime as DATE) as int) >= @fromN and
	CAST(GETDATE() - CAST(cv.InTime as DATE) as int) <= @toN)
	and (c.companyid = @companyid or @companyid is null)
	and (c.clubid = @divisionid or @divisionid is null)

order by [Число дней с последнего посещения]
end

