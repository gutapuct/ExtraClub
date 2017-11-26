
CREATE procedure [dbo].[rep_Anthropometrics] (@start DateTime, 
@end DateTime, @companyid uniqueidentifier, @divisionid uniqueidentifier, @showall bit)as
	begin

select c.id _customerId, d.name Клуб, c.LastName Фамилия, c.FirstName Имя, c.MiddleName Отчество, 
		STUFF(( SELECT ', ' + st.name AS [text()]
		FROM dbo.CustomersCustomerStatuses cst
		inner join dbo.CustomerStatuses st on st.Id=cst.CustomerStatusId
		WHERE
		cst.CustomerId=c.id
		FOR XML PATH('')
		), 1, 1, '' ) Статус,
		c.Phone2 Телефон,
		a.CreatedOn Дата, u.FullName сотрудник, a.Height Рост, a.Weight Вес, a.PSPulse [PS Пульс], a.ADUp [AD Верхнее],
		a.ADDown [AD Нижнее], a.Neck Шея, a.ChestIn [Грудь вдох], a.ChestOut [Грудь выдох], a.RightRelax [Правое плечо расслаблено], a.RightTense [Правое плечо напряжено], a.LeftRelax [Левое плечо расслаблено],
		a.LeftTense [Левое плечо напряжено], a.ForearmRight [Предплечье правое], a.ForearmLeft [Предплечье левое], a.Waist [Талия], a.Stomach Живот, a.Leg Бедра, a.Buttocks Ягодицы,
		a.LegRight [Бедро правое], a.LegLeft [Бедро левое], a.ShinRight [Голень правая], a.ShinLeft [Голень левая]
from customers c

cross apply (select top 1 id from Anthropometrics a
                where a.customerid = c.id 
				order by createdon
				union
				select top 1 id from Anthropometrics a
                where a.customerid = c.id and @showall=1
				order by createdon desc
            ) ca

inner join Anthropometrics a on (@showall = 1 and  ca.id = a.id) or (@showall = 0 and a.customerid=c.id)
inner join divisions d on c.ClubId = d.Id
inner join companies comp on comp.companyid = a.companyid
inner join users u on u.UserId = a.CreatedBy

where (((a.CreatedOn > @start) and (a.CreatedOn < dateadd (day, 1, @end))) or (@showall=1))
and (c.companyid=@companyid or @companyid is null)
and (c.clubid = @divisionid or @divisionid is null)

order by c.lastname, Дата
end

