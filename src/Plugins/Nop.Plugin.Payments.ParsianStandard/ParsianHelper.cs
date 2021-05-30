﻿namespace Nop.Plugin.Payments.ParsianStandard
{
    public class ParsianHelper
    {
        #region Properties

        /// <summary>
        /// Get ferasatsystem partner code
        /// </summary>
        public static string FerasatSystemPartnerCode => "ferasatsystem_SP";

        /// <summary>
        /// Get the generic attribute name that is used to store an order total that actually sent to Parsian (used to PDT order total validation)
        /// </summary>
        public static string OrderTotalSentToParsian => "OrderTotalSentToParsian";

        #endregion

        #region Methods

        /// <summary>
        /// Gets a payment status
        /// </summary>
        /// <param name="paymentStatus">Parsian payment status</param>
        /// <param name="pendingReason">Parsian pending reason</param>
        /// <returns>Payment status</returns>
        public static PaymentStatus GetPaymentStatus(string paymentStatus,
            string pendingReason)
        {
            var result = PaymentStatus.Pending;

            if (paymentStatus == null)
                paymentStatus = string.Empty;

            if (pendingReason == null)
                pendingReason = string.Empty;

            switch (paymentStatus.ToLowerInvariant())
            {
                case "pending":
                    switch (pendingReason.ToLowerInvariant())
                    {
                        case "authorization":
                            result = PaymentStatus.Authorized;
                            break;
                        default:
                            result = PaymentStatus.Pending;
                            break;
                    }

                    break;
                case "processed":
                case "completed":
                case "canceled_reversal":
                    result = PaymentStatus.Paid;
                    break;
                case "denied":
                case "expired":
                case "failed":
                case "voided":
                    result = PaymentStatus.Voided;
                    break;
                case "refunded":
                case "reversed":
                    result = PaymentStatus.Refunded;
                    break;
                default:
                    break;
            }

            return result;
        }

        #endregion
    }
}