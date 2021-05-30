CREATE TABLE PartialPaymentUsageHistory
(
    Id               int primary key identity (1,1),
    CustomerId       int FOREIGN KEY REFERENCES [dbo].[Customer] (Id),
    OrderId          int FOREIGN KEY REFERENCES [dbo].[Order] (Id),
    SpentValue       decimal(18, 4),
    CreatedOn        datetime2(7) not null
)