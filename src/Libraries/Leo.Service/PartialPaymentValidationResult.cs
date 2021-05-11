using System.Collections.Generic;

namespace Leo.Service
{
    public class PartialPaymentValidationResult
    {
        public PartialPaymentValidationResult()
        {
            Errors = new List<string>();
        }
        public bool IsValid { get; set; }

        public IList<string> Errors { get; set; }
    }
}