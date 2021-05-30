namespace Nop.Plugin.Payments.ParsianStandard.Models
{
    public class SalePaymentRequest
    {
        public static string Url =>
            "https://pec.shaparak.ir/NewIPGServices/Sale/SaleService.asmx";
    }

    /// <summary>
    /// Name of the variable of this type should be : requestData
    /// <example>var requestData=new ClientSalePaymentRequestData()</example>
    /// </summary>
    public class ClientSalePaymentRequestData
    {
        /// <summary>
        /// Every store has a PIN number
        /// </summary>
        public string LoginAccount { get; set; }

        /// <summary>
        /// Amount of money customer must pay
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// order id of which the customer is paying about
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// callback url after payment is done
        /// </summary>
        public string CallBackUrl { get; set; }
    }

    public class ClientSalePaymentResponseData
    {
        /// <summary>
        /// Request number in Gateway payment which is a random and unique
        /// for all transaction process
        /// If greater than ZERO it mean it's valid.
        /// </summary>
        public long Token { get; set; }

        /// <summary>
        /// if success equals to ZERO
        /// </summary>
        public short Status { get; set; }
    }
}