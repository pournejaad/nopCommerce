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
            //order_summary_content_deals
            //op_checkout_confirm_top
            
            return new List<string>() {"op_checkout_confirm_bottom"};
        }

        public string GetWidgetViewComponentName(string widgetZone)
        {
            return "WidgetsCustomerWallet";
        }
    }
}