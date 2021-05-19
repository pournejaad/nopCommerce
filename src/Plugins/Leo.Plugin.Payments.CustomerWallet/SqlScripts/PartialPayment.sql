create table PartialPayment
(
    Id                          int primary key identity(1,1),
    Name                        nvarchar(128) not null,
    AdminComment                nvarchar(max) null,
    UsePercentage               bit            not null,
    PartialPaymentPercentage    decimal(18, 4) not null,
    PartialPaymentAmount        decimal(18, 4) not null,
    MaximumPartialPaymentAmount decimal(18, 4) null,
    StartDateUtc                datetime2(7) null,
    EndDateUtc                  datetime2(7) null
)