using Nop.Core.Configuration;

namespace Nop.Plugin.Payments.ParsianStandard
{
    /// <summary>
    /// Represents settings of the Parsian payment plugin
    /// </summary>
    public class ParsianPaymentSettings : ISettings
    {
        public string PIN { get; set; }
    }
}