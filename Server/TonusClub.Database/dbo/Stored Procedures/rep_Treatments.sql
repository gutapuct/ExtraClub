
CREATE procedure [dbo].[rep_Treatments] (@franchid uniqueidentifier, @divisionid uniqueidentifier, @companyid uniqueidentifier) as
begin

declare @t table(ttid uniqueidentifier, coun int)
insert into @t
select t.ThreatmentTypeId, COUNT(*) from Claims c
inner join treatments t on c.Eq_TreatmentId = t.Id
group by t.ThreatmentTypeId

declare @t1 table(tid uniqueidentifier, coun int)
insert into @t1
select t.Id, COUNT(*) from Claims c
inner join treatments t on c.Eq_TreatmentId = t.Id
where (t.CompanyId=@franchid or @franchid is null)
and (t.DivisionId =@divisionid or @divisionid is null)
group by t.Id

select c.CompanyName Компания, d.Name Клуб, tt.Name Тип, t.ModelName Наименование, 
t.DogNumber Договор, t.SerialNumber [Серийный номер], t.Delivery [Дата поставки], t.GuaranteeExp Гарантия,
case t.IsActive when 1 then 'Да' else 'Нет' end Активность,
stat1.coun [Обращений],
stat.coun [Обращений по типу]
from treatments t
inner join TreatmentTypes tt on tt.Id=t.ThreatmentTypeId
inner join Companies c on c.CompanyId=t.CompanyId
inner join Divisions d on d.Id=t.DivisionId
left join @t stat on stat.ttid=tt.Id
left join @t1 stat1 on stat1.tid=t.Id
where (t.CompanyId=@franchid or @franchid is null)
and (t.DivisionId =@divisionid or @divisionid is null)
order by 1,2,3
end

