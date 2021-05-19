using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Leo.Plugin.Payments.CustomerWallet.Models
{
    public class PartialPaymentOptionListTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (!(value is string))
                return base.ConvertFrom(context, culture, value);

            var valueStr = value as string;

            if (string.IsNullOrEmpty(valueStr))
                return null;

            List<PartialPaymentOption> partialPaymentOptions = null;

            try
            {
                using var tr = new StringReader(valueStr);
                var xmlS = new XmlSerializer(typeof(List<PartialPaymentOption>));
                partialPaymentOptions = (List<PartialPaymentOption>)xmlS.Deserialize(tr);
            }
            catch
            {
                //XML error
            }

            return partialPaymentOptions;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType != typeof(string))
                return base.ConvertTo(context, culture, value, destinationType);

            if (!(value is List<PartialPaymentOption>))
                return string.Empty;

            var sb = new StringBuilder();
            using var tw = new StringWriter(sb);
            var xmlS = new XmlSerializer(typeof(List<PartialPaymentOption>));
            xmlS.Serialize(tw, value);
            var serialized = sb.ToString();
            return serialized;
        }
    }
}