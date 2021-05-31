using System.Collections.Generic;
using Nop.Services.Cms;
using Nop.Services.Plugins;

namespace Leo.Plugin.Widgets.CustomerWallet
{
    public class CustomerWalletWidget : BasePlugin, IWidgetPlugin
    {
        public bool HideInWidgetList => false;

        public IList<string> GetWidgetZones()
        {
            return new List<string>() {"order_summary_content_after"};
        }

        public string GetWidgetViewComponentName(string widgetZone)
        {
            return "WidgetsCustomerWallet";
        }
        
    }
}