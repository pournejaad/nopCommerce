create table  PartialPaymentProductMapping
(
    Id int primary key identity(1,1),
    PartialPaymentId int not null foreign key references PartialPayment(Id),
    ProductId int not null foreign key references Product(Id)
)