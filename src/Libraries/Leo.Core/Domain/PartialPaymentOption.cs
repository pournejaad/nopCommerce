namespace Leo.Core.Domain
{
    public class PartialPaymentOption
    {
        public int ShoppingCartItemId { get; set; }
        public decimal AppliedValue { get; set; }
        public decimal AllowedValue { get; set; }
        public bool IsPaid { get; set; }
    }
}