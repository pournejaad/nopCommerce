using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Nop.Services.Plugins;
using Nop.Web.Framework.Menu;

namespace Leo.Plugin.Payments.CustomerWallet
{
    public class CustomerWalletPlugin : BasePlugin,IAdminMenuPlugin
    {
        public Task ManageSiteMapAsync(SiteMapNode rootNode)
        {
            var menuItem = new SiteMapNode()
            {
                SystemName = "Payments.CustomerWallet",
                Title = "Customer Wallet",
                ControllerName = "PartialPayment",
                ActionName = "List",
                Visible = true,
                RouteValues = new RouteValueDictionary() { { "area", "admin" } },
 
            };
            var pluginNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Payments.CustomerWallet");
            if (pluginNode != null)
                pluginNode.ChildNodes.Add(menuItem);
            else
                rootNode.ChildNodes.Add(menuItem);

            return Task.CompletedTask;
        }
    }
}
