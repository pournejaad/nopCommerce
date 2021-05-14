namespace Leo.Core.Payments
{
    public class PartialPaymentOption
    {
        public int ShoppingCarteItemId { get; set; }
        public decimal AppliedValue { get; set; }
        public decimal AllowedValue { get; set; }
        public bool IsPaid { get; set; }
    }
}