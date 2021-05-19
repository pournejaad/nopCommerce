INSERT INTO PermissionRecord([Name],[SystemName],[Category])
VALUES ('Manage Partial Payments','ManagePartialPayments','Promo');

DECLARE @PermissionRecordId INT = (SELECT TOP 1 Id FROM PermissionRecord ORDER BY Id DESC)

DECLARE @AdminRoleId INT = (SELECT Id FROM CustomerRole WHERE Name LIKE '%Admin%')

INSERT INTO
PermissionRecord_Role_Mapping(PermissionRecord_Id, CustomerRole_Id)
VALUES (@PermissionRecordId,@AdminRoleId)