using System;
using System.Collections.Generic;
using Nop.Services.Cms;
using Nop.Services.Plugins;

namespace Leo.Plugin.Wallets.CustomerWallet
{
    public class CustomerWalletWidget : BasePlugin, IWidgetPlugin
    {
        public bool HideInWidgetList => false;

        public IList<string> GetWidgetZones()
        {
            return new List<string>() {"opc_before_confirm"};
        }

        public string GetWidgetViewComponentName(string widgetZone)
        {
            return "CustomerWalletWidget";
        }
    }
}