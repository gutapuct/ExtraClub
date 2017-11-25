Create procedure rep_SalesDeposits (@companyid uniqueidentifier) as
begin
SELECT isnull(c.LastName+' ', '')+ isnull(c.firstname+ ' ', '') + isnull(c.middlename,''), sum (Amount)
FROM DepositAccounts d
INNER JOIN customers c ON c.id = d.CustomerId
where (c.companyid=@companyid or @companyid is null)
GROUP BY c.id, isnull(c.LastName+' ', '')+ isnull(c.firstname+ ' ', '') + isnull(c.middlename,'')
ORDER BY 1
end
