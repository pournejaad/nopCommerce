create table Wallet
(
    Id int primary key identity(1,1),
    CustomerId int foreign key references Customer(Id),
    Balance decimal(18,4) not null
)
