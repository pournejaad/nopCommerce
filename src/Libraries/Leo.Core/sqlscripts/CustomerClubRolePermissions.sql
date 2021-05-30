INSERT INTO PermissionRecord([Name], [SystemName], [Category])
VALUES ('Use Customer Club Wallet', 'UseCustomerClubWallet', 'Customers')

DECLARE @PermissionRecordId INT = (SELECT TOP 1 Id
                                   FROM PermissionRecord
                                   ORDER BY Id DESC)

DECLARE @CustomerClubMemberRoleId INT = (SELECT Id
                                         FROM CustomerRole
                                         WHERE SystemName LIKE '%CustomerClubs%')

INSERT INTO PermissionRecord_Role_Mapping(PermissionRecord_Id, CustomerRole_Id)
VALUES (@PermissionRecordId, @CustomerClubMemberRoleId)