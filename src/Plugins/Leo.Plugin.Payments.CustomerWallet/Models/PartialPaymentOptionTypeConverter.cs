using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Leo.Plugin.Payments.CustomerWallet.Models
{
    public class PartialPaymentOptionTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context,
            Type sourceType)
        {
            return sourceType == typeof(string) ||
                   base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context,
            CultureInfo culture, object value)
        {
            if (value is not string valueStr)
            {
                return base.ConvertFrom(context, culture, value);
            }

            if (string.IsNullOrEmpty(valueStr))
            {
                return null;
            }

            PartialPaymentOption partialPaymentOption = null;
            try
            {
                using var tr = new StringReader(valueStr);
                var xmlS = new XmlSerializer(typeof(PartialPaymentOption));
                partialPaymentOption = (PartialPaymentOption)xmlS.Deserialize(tr);
            }
            catch
            {
                // 
            }

            return partialPaymentOption;
        }
        
        
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType != typeof(string)) 
                return base.ConvertTo(context, culture, value, destinationType);

            if (!(value is PartialPaymentOption)) 
                return string.Empty;

            var sb = new StringBuilder();
            using var tw = new StringWriter(sb);
            var xmlS = new XmlSerializer(typeof(PartialPaymentOption));
            xmlS.Serialize(tw, value);
            var serialized = sb.ToString();
            return serialized;
        }
    }
}