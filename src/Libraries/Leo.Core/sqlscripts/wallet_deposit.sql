CREATE TABLE CustomerWalletCharge
(
    [Id]       INT PRIMARY KEY IDENTITY (1,1),
    Amount     decimal(18, 4) NOT NULL,
    CustomerId INT foreign key references Customer (Id),
    CreatedAt  datetime2(7) default GETDATE()
)

