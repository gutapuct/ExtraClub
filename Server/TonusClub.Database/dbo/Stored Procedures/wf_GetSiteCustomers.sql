
CREATE procedure [dbo].[wf_GetSiteCustomers]
as
select sf.Email, '+7'+replace(replace(replace(replace(sf.Phone,'(',''),')',''),' ',''),'-','') Phone, 
sf.Name, cp.ClubId, d.CompanyId from slendercalc..SubmitedForms sf
inner join TcSite..ClubPages cp on cp.ClubName=sf.City
inner join Divisions d on d.Id=cp.ClubId
where Date>dateadd(day, -1, getdate())
and not exists (select * from customers c where c.Phone2=sf.Phone and c.CompanyId=d.CompanyId)
and ShareData = 1
and 1=0 --no need in ths method
