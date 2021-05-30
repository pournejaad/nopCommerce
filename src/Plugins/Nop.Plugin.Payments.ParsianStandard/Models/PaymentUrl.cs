namespace Nop.Plugin.Payments.ParsianStandard.Models
{
    public class PaymentUrl
    {
        public static string FormattedUrlWithToken =>
            "https://pec.shaparak.ir/NewIPG/?Token={0}";
    }


    public class ConfirmPayment
    {
        public static string Url =>
            "https://pec.shaparak.ir/NewIPGServices/Confirm/ConfirmService.asmx";
    }

    /// <summary>
    /// 
    /// </summary>
    public class ClientConfirmRequestData
    {
        public long LoginAccount { get; set; }
        public long Token { get; set; }
    }

    public class ClientConfirmResponseData
    {
        public short Status { get; set; }
        public string RRN { get; set; }
        public string CardNumberMasked { get; set; }
        public long Token { get; set; }
    }
}