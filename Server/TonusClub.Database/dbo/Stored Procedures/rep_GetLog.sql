

CREATE procedure [dbo].[rep_GetLog](@userName nvarchar(250), @start datetime, @end datetime, @companyId uniqueidentifier)
as
select isnull(fullname, log.username) Пользователь, HostIp Компьютер, Message Действие, Date Время from log
left join users u on u.username=log.username
where date>=@start and date<dateadd(day, 1, @end)
and log.username like '%'+isnull(@userName,'')+'%'
and (u.companyid=@companyid or @companyid is null)
order by date desc
