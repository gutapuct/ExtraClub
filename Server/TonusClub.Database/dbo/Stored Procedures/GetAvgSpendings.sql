CREATE procedure [dbo].[GetAvgSpendings](@start date, @end date, @divisionId uniqueidentifier)
as

select avg(res) from(
select cast(sum(uc.UnitCharge) as money) res from UnitCharges uc
inner join Tickets t on t.Id=uc.TicketId
where t.DivisionId=@divisionId and uc.[Date]>=@start and uc.[Date] <@end
group by DATEADD(dd, 0, DATEDIFF(dd, 0, uc.[Date])), CustomerId) as proj1