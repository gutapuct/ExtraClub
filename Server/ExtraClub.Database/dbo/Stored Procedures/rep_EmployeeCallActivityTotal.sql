
CREATE procedure rep_EmployeeCallActivityTotal (@start datetime, @end datetime, @companyId uniqueidentifier)
as
select u.FullName Сотрудник, cast(c.StartAt as DATE) Дата, COUNT(*) [Всего контактов] from calls c
inner join Users u on u.UserId=c.CreatedBy
where (@companyId is null or @companyId=c.CompanyId)
and (StartAt>=@start or @start is null)
and (dateadd(day, 1, StartAt)<@end or @end is null)
group by u.FullName, cast(c.StartAt as DATE)
order by 1,2

