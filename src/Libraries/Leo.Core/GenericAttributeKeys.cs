using System.Collections.Generic;
using System.ComponentModel;
using Leo.Core.ComponentModel;
using Leo.Core.Domain;
using Nop.Core.Infrastructure;

namespace Leo.Core
{
    public class GenericAttributeKeys
    {
        public static string PartialPayment => "PartialPaymentOption";
    }

    public class TypeConverterRegistrationStartupTask : IStartupTask
    {
        public void Execute()
        {
            TypeDescriptor.AddAttributes(typeof(PartialPaymentOption),
                new TypeConverterAttribute(
                    typeof(PartialPaymentOptionTypeConverter)));
            TypeDescriptor.AddAttributes(typeof(List<PartialPaymentOption>), new TypeConverterAttribute(typeof(PartialPaymentOptionListTypeConverter)));
            TypeDescriptor.AddAttributes(typeof(IList<PartialPaymentOption>), new TypeConverterAttribute(typeof(PartialPaymentOptionListTypeConverter)));

        }

        public int Order => 1;
    }
}
