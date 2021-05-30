using System.Collections.Generic;
using System.Linq;

namespace Nop.Plugin.Payments.ParsianStandard.Helpers
{
    internal static class HtmlFormBuilder
    {
        public const string FORM_ID = "paymentForm";
        public static string CreateForm(string url, IEnumerable<KeyValuePair<string, string>> data)
        {
            var fields = string.Join("", data.Select(item => CreateHiddenInput(item.Key, item.Value)));

            return
                "<html>" +
                "<body>" +
                $"<form id=\"{FORM_ID}\" action=\"{url}\" method=\"post\" />" +
                fields +
                "</form>" +
                "<script type=\"text/javascript\">" +
                "document.getElementById('paymentForm').submit();" +
                "</script>" +
                "</body>" +
                "</html>";
        }

        public static string CreateHiddenInput(string name, string value)
        {
            return $"<input type=\"hidden\" name=\"{name}\" value=\"{value}\" />";
        }
    }
}
