using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Leo.Core.Payments;

namespace Leo.Core
{
    public class TypeConverterRegistrationStartupTask : Nop.Core.Infrastructure.IStartupTask
    {
        public Task ExecuteAsync()
        {
            TypeDescriptor.AddAttributes(typeof(PartialPaymentOption), new TypeConverterAttribute(typeof(PartialPaymentOptionTypeConverter)));
            TypeDescriptor.AddAttributes(typeof(List<PartialPaymentOption>), new TypeConverterAttribute(typeof(PartialPaymentOptionListTypeConverter)));
            TypeDescriptor.AddAttributes(typeof(IList<PartialPaymentOption>), new TypeConverterAttribute(typeof(PartialPaymentOptionListTypeConverter)));
            return Task.CompletedTask;
        }

        public int Order => 1;
    }
}