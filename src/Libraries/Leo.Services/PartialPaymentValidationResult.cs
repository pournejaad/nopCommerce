using System.Collections.Generic;

namespace Leo.Services
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