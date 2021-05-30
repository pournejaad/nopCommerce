CREATE TABLE CustomerWalletDeposit
(
    Id         INT PRIMARY KEY IDENTITY (1,1),
    Source     nvarchar(256)  not null,
    Amount     decimal(18, 4) not null,
    CustomerId int foreign key references Customer (Id),
    CreatedOn  datetime2(7) default GETDATE(),
)

