CREATE procedure [dbo].[sync_GetServerChanges](@company uniqueidentifier, @version bigint)
as
 
 
SELECT 'ProviderFolders', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES ProviderFolders, @version) AS c
LEFT OUTER JOIN ProviderFolders AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'DivisionFinances', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES DivisionFinances, @version) AS c
LEFT OUTER JOIN DivisionFinances AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'Tickets', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES Tickets, @version) AS c
LEFT OUTER JOIN Tickets AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'SalaryRateTables', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES SalaryRateTables, @version) AS c
LEFT OUTER JOIN SalaryRateTables AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'GoodActions', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES GoodActions, @version) AS c
LEFT OUTER JOIN GoodActions AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'CustomerStatuses', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES CustomerStatuses, @version) AS c
LEFT OUTER JOIN CustomerStatuses AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'TreatmentPrograms', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES TreatmentPrograms, @version) AS c
LEFT OUTER JOIN TreatmentPrograms AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'IncomeTypes', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES IncomeTypes, @version) AS c
LEFT OUTER JOIN IncomeTypes AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'CashInOrders', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES CashInOrders, @version) AS c
LEFT OUTER JOIN CashInOrders AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'CustomersCustomerStatuses', c.CustomerStatusId,c.CustomerId, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES CustomersCustomerStatuses, @version) AS c
--LEFT OUTER JOIN CustomersCustomerStatuses AS e  with (nolock) ON e.CustomerStatusId=c.CustomerStatusId and e.CustomerId=c.CustomerId  
 
 
SELECT 'Nutritions', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES Nutritions, @version) AS c
LEFT OUTER JOIN Nutritions AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'OrganizationTypes', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES OrganizationTypes, @version) AS c
--LEFT OUTER JOIN OrganizationTypes AS e  with (nolock) ON e.Id=c.Id  
 
 
SELECT 'Goods', c.GoodId, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES Goods, @version) AS c
LEFT OUTER JOIN Goods AS e  with (nolock) ON e.GoodId=c.GoodId 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'SalarySheets', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES SalarySheets, @version) AS c
LEFT OUTER JOIN SalarySheets AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'CashOutOrders', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES CashOutOrders, @version) AS c
LEFT OUTER JOIN CashOutOrders AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'CustomerMeasures', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES CustomerMeasures, @version) AS c
LEFT OUTER JOIN CustomerMeasures AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'SpendingTypes', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES SpendingTypes, @version) AS c
LEFT OUTER JOIN SpendingTypes AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'TreatmentConfig', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES TreatmentConfig, @version) AS c
 
 
SELECT 'SalarySheetRows', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES SalarySheetRows, @version) AS c
LEFT OUTER JOIN SalarySheetRows AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'UnitCharges', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES UnitCharges, @version) AS c
LEFT OUTER JOIN UnitCharges AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'GoodPrices', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES GoodPrices, @version) AS c
LEFT OUTER JOIN GoodPrices AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'Storehouses', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES Storehouses, @version) AS c
LEFT OUTER JOIN Storehouses AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'CustomerCardTypesTicketTypes', c.CustomerCardTypeId,c.TicketTypeId, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES CustomerCardTypesTicketTypes, @version) AS c
--LEFT OUTER JOIN CustomerCardTypesTicketTypes AS e  with (nolock) ON e.CustomerCardTypeId=c.CustomerCardTypeId and e.TicketTypeId=c.TicketTypeId  
 
 
SELECT 'Certificates', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES Certificates, @version) AS c
LEFT OUTER JOIN Certificates AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'MinutesCharges', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES MinutesCharges, @version) AS c
LEFT OUTER JOIN MinutesCharges AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'SettingsFolders', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES SettingsFolders, @version) AS c
--LEFT OUTER JOIN SettingsFolders AS e  with (nolock) ON e.Id=c.Id  
 
 
SELECT 'Divisions', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES Divisions, @version) AS c
LEFT OUTER JOIN Divisions AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'CustomerCards', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES CustomerCards, @version) AS c
LEFT OUTER JOIN CustomerCards AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'BonusAccounts', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES BonusAccounts, @version) AS c
LEFT OUTER JOIN BonusAccounts AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'GoodSales', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES GoodSales, @version) AS c
LEFT OUTER JOIN GoodSales AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'CumulativeDiscounts', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES CumulativeDiscounts, @version) AS c
LEFT OUTER JOIN CumulativeDiscounts AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'CustomerCrmEvents', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES CustomerCrmEvents, @version) AS c
LEFT OUTER JOIN CustomerCrmEvents AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'EmployeeCategories', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES EmployeeCategories, @version) AS c
LEFT OUTER JOIN EmployeeCategories AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'ChildrenRoom', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES ChildrenRoom, @version) AS c
LEFT OUTER JOIN ChildrenRoom AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'TicketTypesTreatmentsRestrictions', c.TicketTypeId,c.TreatmentTypeId, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES TicketTypesTreatmentsRestrictions, @version) AS c
--LEFT OUTER JOIN TicketTypesTreatmentsRestrictions AS e  with (nolock) ON e.TicketTypeId=c.TicketTypeId and e.TreatmentTypeId=c.TreatmentTypeId  
 
 
SELECT 'EmployeePayments', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES EmployeePayments, @version) AS c
LEFT OUTER JOIN EmployeePayments AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'Spendings', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES Spendings, @version) AS c
LEFT OUTER JOIN Spendings AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'Consignments', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES Consignments, @version) AS c
LEFT OUTER JOIN Consignments AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'Jobs', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES Jobs, @version) AS c
LEFT OUTER JOIN Jobs AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'ConsignmentLines', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES ConsignmentLines, @version) AS c
LEFT OUTER JOIN ConsignmentLines AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'Permissions', c.PermissionId, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES Permissions, @version) AS c
--LEFT OUTER JOIN Permissions AS e  with (nolock) ON e.PermissionId=c.PermissionId  
 
 
SELECT 'Roles', c.RoleId, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES Roles, @version) AS c
LEFT OUTER JOIN Roles AS e  with (nolock) ON e.RoleId=c.RoleId 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'BarOrders', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES BarOrders, @version) AS c
LEFT OUTER JOIN BarOrders AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'CustomerNotifications', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES CustomerNotifications, @version) AS c
LEFT OUTER JOIN CustomerNotifications AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'Companies_SettingFolders', c.CompanyId,c.SettingsFolderId, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES Companies_SettingFolders, @version) AS c
LEFT OUTER JOIN Companies_SettingFolders AS e  with (nolock) ON e.CompanyId=c.CompanyId and e.SettingsFolderId=c.SettingsFolderId 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'Corporates', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES Corporates, @version) AS c
LEFT OUTER JOIN Corporates AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'Users', c.UserId, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES Users, @version) AS c
LEFT OUTER JOIN Users AS e  with (nolock) ON e.UserId=c.UserId 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'UnitTypes', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES UnitTypes, @version) AS c
--LEFT OUTER JOIN UnitTypes AS e  with (nolock) ON e.Id=c.Id  
 
 
SELECT 'JobsCategories', c.JobId,c.CategoryId, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES JobsCategories, @version) AS c
--LEFT OUTER JOIN JobsCategories AS e  with (nolock) ON e.JobId=c.JobId and e.CategoryId=c.CategoryId  
 
 
SELECT 'GoodsCategories', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES GoodsCategories, @version) AS c
--LEFT OUTER JOIN GoodsCategories AS e  with (nolock) ON e.Id=c.Id  
 
 
SELECT 'Manufacturers', c.ManufacturerId, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES Manufacturers, @version) AS c
LEFT OUTER JOIN Manufacturers AS e  with (nolock) ON e.ManufacturerId=c.ManufacturerId 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'ProductTypes', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES ProductTypes, @version) AS c
--LEFT OUTER JOIN ProductTypes AS e  with (nolock) ON e.Id=c.Id  
 
 
SELECT 'CustomerShelves', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES CustomerShelves, @version) AS c
LEFT OUTER JOIN CustomerShelves AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'Customers', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES Customers, @version) AS c
LEFT OUTER JOIN Customers AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'RolesPermissions', c.Permissions_PermissionId,c.Roles_RoleId, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES RolesPermissions, @version) AS c
--LEFT OUTER JOIN RolesPermissions AS e  with (nolock) ON e.Permissions_PermissionId=c.Permissions_PermissionId and e.Roles_RoleId=c.Roles_RoleId  
 
 
SELECT 'UsersRoles', c.Roles_RoleId,c.Users_UserId, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES UsersRoles, @version) AS c
--LEFT OUTER JOIN UsersRoles AS e  with (nolock) ON e.Roles_RoleId=c.Roles_RoleId and e.Users_UserId=c.Users_UserId  
 
 
SELECT 'IncomingCallForms', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES IncomingCallForms, @version) AS c
--LEFT OUTER JOIN IncomingCallForms AS e  with (nolock) ON e.Id=c.Id  
 
 
SELECT 'DepositAccounts', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES DepositAccounts, @version) AS c
LEFT OUTER JOIN DepositAccounts AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'Tasks', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES Tasks, @version) AS c
LEFT OUTER JOIN Tasks AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'ProviderPayments', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES ProviderPayments, @version) AS c
LEFT OUTER JOIN ProviderPayments AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'IncomingCallFormButtons', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES IncomingCallFormButtons, @version) AS c
--LEFT OUTER JOIN IncomingCallFormButtons AS e  with (nolock) ON e.Id=c.Id  
 
 
SELECT 'Employees', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES Employees, @version) AS c
LEFT OUTER JOIN Employees AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'TreatmentProgramLines', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES TreatmentProgramLines, @version) AS c
LEFT OUTER JOIN TreatmentProgramLines AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'SalesPlans', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES SalesPlans, @version) AS c
LEFT OUTER JOIN SalesPlans AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'JobPlacements', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES JobPlacements, @version) AS c
LEFT OUTER JOIN JobPlacements AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'TreatmentEvents', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES TreatmentEvents, @version) AS c
LEFT OUTER JOIN TreatmentEvents AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'Solariums', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES Solariums, @version) AS c
LEFT OUTER JOIN Solariums AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'CustomReports', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES CustomReports, @version) AS c
LEFT OUTER JOIN CustomReports AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'TicketTypes', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES TicketTypes, @version) AS c
--LEFT OUTER JOIN TicketTypes AS e  with (nolock) ON e.Id=c.Id  
 
 
SELECT 'Calls', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES Calls, @version) AS c
LEFT OUTER JOIN Calls AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'Kinds1C', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES Kinds1C, @version) AS c
--LEFT OUTER JOIN Kinds1C AS e  with (nolock) ON e.Id=c.Id  
 
 
SELECT 'CustomReportsRoles', c.CustomReportId,c.RoleId, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES CustomReportsRoles, @version) AS c
--LEFT OUTER JOIN CustomReportsRoles AS e  with (nolock) ON e.CustomReportId=c.CustomReportId and e.RoleId=c.RoleId  
 
 
SELECT 'Incomes', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES Incomes, @version) AS c
LEFT OUTER JOIN Incomes AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'SolariumMessages', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES SolariumMessages, @version) AS c
--LEFT OUTER JOIN SolariumMessages AS e  with (nolock) ON e.Id=c.Id  
 
 
SELECT 'Files', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES Files, @version) AS c
LEFT OUTER JOIN Files AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'EmployeeVacations', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES EmployeeVacations, @version) AS c
LEFT OUTER JOIN EmployeeVacations AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'AdvertTypes', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES AdvertTypes, @version) AS c
LEFT OUTER JOIN AdvertTypes AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'SavedReports', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES SavedReports, @version) AS c
LEFT OUTER JOIN SavedReports AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'SolariumVisits', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES SolariumVisits, @version) AS c
LEFT OUTER JOIN SolariumVisits AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'Companies', c.CompanyId, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES Companies, @version) AS c
LEFT OUTER JOIN Companies AS e  with (nolock) ON e.CompanyId=c.CompanyId 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'CustomerTargetTypesTreatmentConfigs', c.CustomerTargetTypeId,c.TreatmentConfigId, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES CustomerTargetTypesTreatmentConfigs, @version) AS c
--LEFT OUTER JOIN CustomerTargetTypesTreatmentConfigs AS e  with (nolock) ON e.CustomerTargetTypeId=c.CustomerTargetTypeId and e.TreatmentConfigId=c.TreatmentConfigId  
 
 
SELECT 'TreatmentTypes', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES TreatmentTypes, @version) AS c
--LEFT OUTER JOIN TreatmentTypes AS e  with (nolock) ON e.Id=c.Id  
 
 
SELECT 'EmployeeTrips', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES EmployeeTrips, @version) AS c
LEFT OUTER JOIN EmployeeTrips AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'BarDiscounts', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES BarDiscounts, @version) AS c
LEFT OUTER JOIN BarDiscounts AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'TreatmentsParalleling', c.TreatmentType1Id,c.TreatmentType2Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES TreatmentsParalleling, @version) AS c
--LEFT OUTER JOIN TreatmentsParalleling AS e  with (nolock) ON e.TreatmentType1Id=c.TreatmentType1Id and e.TreatmentType2Id=c.TreatmentType2Id  
 
 
SELECT 'Claims', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES Claims, @version) AS c
LEFT OUTER JOIN Claims AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'TicketTypeLimits', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES TicketTypeLimits, @version) AS c
--LEFT OUTER JOIN TicketTypeLimits AS e  with (nolock) ON e.Id=c.Id  
 
 
SELECT 'EmployeeDocuments', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES EmployeeDocuments, @version) AS c
LEFT OUTER JOIN EmployeeDocuments AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'ContraIndications', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES ContraIndications, @version) AS c
--LEFT OUTER JOIN ContraIndications AS e  with (nolock) ON e.Id=c.Id  
 
 
SELECT 'Ankets', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES Ankets, @version) AS c
LEFT OUTER JOIN Ankets AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'Treatments', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES Treatments, @version) AS c
LEFT OUTER JOIN Treatments AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'TicketPayments', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES TicketPayments, @version) AS c
LEFT OUTER JOIN TicketPayments AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'CompaniesInstalments', c.CompanyId,c.InstalmentId, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES CompaniesInstalments, @version) AS c
LEFT OUTER JOIN CompaniesInstalments AS e  with (nolock) ON e.CompanyId=c.CompanyId and e.InstalmentId=c.InstalmentId 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'CustomerNotificationsEmployees', c.CustomerNotificationId,c.EmployeeId, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES CustomerNotificationsEmployees, @version) AS c
--LEFT OUTER JOIN CustomerNotificationsEmployees AS e  with (nolock) ON e.CustomerNotificationId=c.CustomerNotificationId and e.EmployeeId=c.EmployeeId  
 
 
SELECT 'ContraIndicationsTreatmentTypes', c.ContraIndicationId,c.TreatmentTypeId, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES ContraIndicationsTreatmentTypes, @version) AS c
--LEFT OUTER JOIN ContraIndicationsTreatmentTypes AS e  with (nolock) ON e.ContraIndicationId=c.ContraIndicationId and e.TreatmentTypeId=c.TreatmentTypeId  
 
 
SELECT 'AnketTickets', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES AnketTickets, @version) AS c
LEFT OUTER JOIN AnketTickets AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'TargetTypeSets', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES TargetTypeSets, @version) AS c
--LEFT OUTER JOIN TargetTypeSets AS e  with (nolock) ON e.Id=c.Id  
 
 
SELECT 'EmployeeVisits', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES EmployeeVisits, @version) AS c
LEFT OUTER JOIN EmployeeVisits AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'CompaniesTicketTypes', c.TicketTypeId,c.CompanyId, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES CompaniesTicketTypes, @version) AS c
LEFT OUTER JOIN CompaniesTicketTypes AS e  with (nolock) ON e.TicketTypeId=c.TicketTypeId and e.CompanyId=c.CompanyId 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'TextActionsDivisions', c.TextActionId,c.DivisionId, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES TextActionsDivisions, @version) AS c
--LEFT OUTER JOIN TextActionsDivisions AS e  with (nolock) ON e.TextActionId=c.TextActionId and e.DivisionId=c.DivisionId  
 
 
SELECT 'TicketFreezeReasons', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES TicketFreezeReasons, @version) AS c
--LEFT OUTER JOIN TicketFreezeReasons AS e  with (nolock) ON e.Id=c.Id  
 
 
SELECT 'AnketTreatments', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES AnketTreatments, @version) AS c
LEFT OUTER JOIN AnketTreatments AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'CompaniesCustomerCardTypes', c.CustomerCardTypeId,c.CompanyId, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES CompaniesCustomerCardTypes, @version) AS c
LEFT OUTER JOIN CompaniesCustomerCardTypes AS e  with (nolock) ON e.CustomerCardTypeId=c.CustomerCardTypeId and e.CompanyId=c.CompanyId 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'ContraIndicationsUsers', c.ContraIndicationId,c.CustomerId, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES ContraIndicationsUsers, @version) AS c
--LEFT OUTER JOIN ContraIndicationsUsers AS e  with (nolock) ON e.ContraIndicationId=c.ContraIndicationId and e.CustomerId=c.CustomerId  
 
 
SELECT 'TicketFreezes', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES TicketFreezes, @version) AS c
LEFT OUTER JOIN TicketFreezes AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'ReportRecurrencies', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES ReportRecurrencies, @version) AS c
LEFT OUTER JOIN ReportRecurrencies AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'VacationLists', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES VacationLists, @version) AS c
LEFT OUTER JOIN VacationLists AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'Rents', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES Rents, @version) AS c
LEFT OUTER JOIN Rents AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'TaskEmployee', c.TaskId,c.EmployeeId, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES TaskEmployee, @version) AS c
--LEFT OUTER JOIN TaskEmployee AS e  with (nolock) ON e.TaskId=c.TaskId and e.EmployeeId=c.EmployeeId  
 
 
SELECT 'CustomerTargets', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES CustomerTargets, @version) AS c
LEFT OUTER JOIN CustomerTargets AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'AnketAdverts', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES AnketAdverts, @version) AS c
LEFT OUTER JOIN AnketAdverts AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'VacationPreferences', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES VacationPreferences, @version) AS c
LEFT OUTER JOIN VacationPreferences AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'TicketCorrections', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES TicketCorrections, @version) AS c
LEFT OUTER JOIN TicketCorrections AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'DictionaryCategories', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES DictionaryCategories, @version) AS c
--LEFT OUTER JOIN DictionaryCategories AS e  with (nolock) ON e.Id=c.Id  
 
 
SELECT 'CustomerCardTypes', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES CustomerCardTypes, @version) AS c
--LEFT OUTER JOIN CustomerCardTypes AS e  with (nolock) ON e.Id=c.Id  
 
 
SELECT 'VacationListItems', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES VacationListItems, @version) AS c
LEFT OUTER JOIN VacationListItems AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'TextActions', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES TextActions, @version) AS c
LEFT OUTER JOIN TextActions AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'ReportInfos', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES ReportInfos, @version) AS c
--LEFT OUTER JOIN ReportInfos AS e  with (nolock) ON e.Id=c.Id  
 
 
SELECT 'TicketTypesShop', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES TicketTypesShop, @version) AS c
LEFT OUTER JOIN TicketTypesShop AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'Anthropometrics', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES Anthropometrics, @version) AS c
LEFT OUTER JOIN Anthropometrics AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'ReportParameters', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES ReportParameters, @version) AS c
--LEFT OUTER JOIN ReportParameters AS e  with (nolock) ON e.Id=c.Id  
 
 
SELECT 'EmployeeWorkGraphs', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES EmployeeWorkGraphs, @version) AS c
LEFT OUTER JOIN EmployeeWorkGraphs AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'AdvertGroups', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES AdvertGroups, @version) AS c
--LEFT OUTER JOIN AdvertGroups AS e  with (nolock) ON e.Id=c.Id  
 
 
SELECT 'TreatmentTypeGroups', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES TreatmentTypeGroups, @version) AS c
--LEFT OUTER JOIN TreatmentTypeGroups AS e  with (nolock) ON e.Id=c.Id  
 
 
SELECT 'News', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES News, @version) AS c
--LEFT OUTER JOIN News AS e  with (nolock) ON e.Id=c.Id  
 
 
SELECT 'DictionaryInfo', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES DictionaryInfo, @version) AS c
--LEFT OUTER JOIN DictionaryInfo AS e  with (nolock) ON e.Id=c.Id  
 
 
SELECT 'DoctorVisits', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES DoctorVisits, @version) AS c
LEFT OUTER JOIN DoctorVisits AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'Holidays', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES Holidays, @version) AS c
--LEFT OUTER JOIN Holidays AS e  with (nolock) ON e.Id=c.Id  
 
 
SELECT 'DepositOuts', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES DepositOuts, @version) AS c
LEFT OUTER JOIN DepositOuts AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'CustomerTargetTypes', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES CustomerTargetTypes, @version) AS c
--LEFT OUTER JOIN CustomerTargetTypes AS e  with (nolock) ON e.Id=c.Id  
 
 
SELECT 'Instalments', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES Instalments, @version) AS c
--LEFT OUTER JOIN Instalments AS e  with (nolock) ON e.Id=c.Id  
 
 
SELECT 'Providers', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES Providers, @version) AS c
LEFT OUTER JOIN Providers AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'SalaryScheme', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES SalaryScheme, @version) AS c
LEFT OUTER JOIN SalaryScheme AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'SshFiles', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES SshFiles, @version) AS c
--LEFT OUTER JOIN SshFiles AS e  with (nolock) ON e.Id=c.Id  
 
 
SELECT 'CompanyFinances', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES CompanyFinances, @version) AS c
LEFT OUTER JOIN CompanyFinances AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'ReportTemplates', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES ReportTemplates, @version) AS c
LEFT OUTER JOIN ReportTemplates AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'CustomerVisits', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES CustomerVisits, @version) AS c
LEFT OUTER JOIN CustomerVisits AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'TreatmentSeqRests', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES TreatmentSeqRests, @version) AS c
--LEFT OUTER JOIN TreatmentSeqRests AS e  with (nolock) ON e.Id=c.Id  
 
 
SELECT 'GoodActionLines', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES GoodActionLines, @version) AS c
LEFT OUTER JOIN GoodActionLines AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
 
 
SELECT 'SalarySchemeCoefficients', c.Id, SYS_CHANGE_OPERATION ChType
FROM CHANGETABLE (CHANGES SalarySchemeCoefficients, @version) AS c
LEFT OUTER JOIN SalarySchemeCoefficients AS e  with (nolock) ON e.Id=c.Id 
where e.companyId=@company or e.companyId is null
Select null,null where 1=2
