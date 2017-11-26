CREATE procedure [dbo].[sp_GetSmsMarketing]
as
declare @frz table(TicketId uniqueidentifier, Freeze int)
insert into @frz
select TicketId, SUM(DATEDIFF(day, StartDate, FinishDate))
from TicketFreezes
group by TicketId

select isnull(UtcCorr, 0) Utc, t.Id TicketId, cu.phone2, cu.id CustomerId
from Divisions d
inner join companies c on c.companyid=d.companyid
inner join syncmetadata.dbo.MetaCompanies mc on d.Id=mc.DivisionId
inner join Tickets t on t.DivisionId=d.Id
inner join TicketTypes tt on tt.Id=t.TicketTypeId
left join @frz tf on tf.TicketId=t.Id
left join unitcharges uc on uc.ticketid=t.id
left join SyncMetadata.dbo.NotifiedMessages n on n.TicketId=t.Id
inner join customers cu on cu.id=t.customerid
where mc.SendSms = 1 and mc.SmsMarketing = 1 and tt.IsSmart = 1 and tt.IsActive=1 and cu.SmsList=1 and n.Id is null
group by d.Id, isnull(UtcCorr, 0), t.id, t.Length, t.StartDate, t.unitsamount, cu.phone2, cu.id, freeze
having (datediff(day, getdate(), dateadd(day, t.length+isnull(freeze, 0), t.startdate)) < 4 or ((t.unitsamount - isnull(sum(uc.unitcharge), 0)) / 8 < 2 ))
and dateadd(day, t.length+isnull(freeze, 0), t.startdate) > getdate()