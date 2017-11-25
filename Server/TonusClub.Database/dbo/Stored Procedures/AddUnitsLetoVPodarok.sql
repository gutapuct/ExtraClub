CREATE procedure [dbo].[AddUnitsLetoVPodarok] (@cid uniqueidentifier, @ttid uniqueidentifier)
as

insert into unitcharges
select newid(),companyid,id, -(used-60), 0 , 'Акция "Лето в подарок"', getdate(), userid, null from (
select Id, companyid, unitsamount, isnull((select SUM(unitcharge) from UnitCharges u where u.TicketId=t.Id and u.date<'01/09/2013'), 0) as used, (select top 1 userid from users u where u.companyid=t.companyid) userid from Tickets t
where (CompanyId=@cid and
 TicketTypeId=@ttid)
and 
CreatedOn>='01/06/2013' and CreatedOn<'16/06/2013'
) as src
where used>60


insert into unitcharges
select newid(),companyid,id, -(used-50), 0 , 'Акция "Лето в подарок"', getdate(), userid, null from (
select Id, companyid, unitsamount, isnull((select SUM(unitcharge) from UnitCharges u where u.TicketId=t.Id and u.date<'01/09/2013'), 0) as used, (select top 1 userid from users u where u.companyid=t.companyid) userid from Tickets t
where (CompanyId=@cid and
 TicketTypeId=@ttid)
and 
CreatedOn>='16/06/2013' and CreatedOn<'01/07/2013'
) as src
where used>50

insert into unitcharges
select newid(),companyid,id, -(used-40), 0 , 'Акция "Лето в подарок"', getdate(), userid, null from (
select Id, companyid, unitsamount, isnull((select SUM(unitcharge) from UnitCharges u where u.TicketId=t.Id and u.date<'01/09/2013'), 0) as used, (select top 1 userid from users u where u.companyid=t.companyid) userid from Tickets t
where (CompanyId=@cid and
 TicketTypeId=@ttid)
and 
CreatedOn>='01/07/2013' and CreatedOn<'16/07/2013'
) as src
where used>40

insert into unitcharges
select newid(),companyid,id, -(used-30), 0 , 'Акция "Лето в подарок"', getdate(), userid, null from (
select Id, companyid, unitsamount, isnull((select SUM(unitcharge) from UnitCharges u where u.TicketId=t.Id and u.date<'01/09/2013'), 0) as used, (select top 1 userid from users u where u.companyid=t.companyid) userid from Tickets t
where (CompanyId=@cid and
 TicketTypeId=@ttid)
and 
CreatedOn>='16/07/2013' and CreatedOn<'01/08/2013'
) as src
where used>30

insert into unitcharges
select newid(),companyid,id, -(used-20), 0 , 'Акция "Лето в подарок"', getdate(), userid, null from (
select Id, companyid, unitsamount, isnull((select SUM(unitcharge) from UnitCharges u where u.TicketId=t.Id and u.date<'01/09/2013'), 0) as used, (select top 1 userid from users u where u.companyid=t.companyid) userid from Tickets t
where (CompanyId=@cid and
 TicketTypeId=@ttid)
and 
CreatedOn>='01/08/2013' and CreatedOn<'16/08/2013'
) as src
where used>20

insert into unitcharges
select newid(),companyid,id, -(used-10), 0 , 'Акция "Лето в подарок"', getdate(), userid, null from (
select Id, companyid, unitsamount, isnull((select SUM(unitcharge) from UnitCharges u where u.TicketId=t.Id and u.date<'01/09/2013'), 0) as used, (select top 1 userid from users u where u.companyid=t.companyid) userid from Tickets t
where (CompanyId=@cid and
 TicketTypeId=@ttid)
and 
CreatedOn>='16/08/2013' and CreatedOn<'01/09/2013'
) as src
where used>10
