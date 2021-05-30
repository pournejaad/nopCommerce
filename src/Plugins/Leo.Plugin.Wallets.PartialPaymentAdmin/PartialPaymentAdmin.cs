using System.Linq;
using Microsoft.AspNetCore.Routing;
using Nop.Services.Plugins;
using Nop.Web.Framework.Menu;

namespace Leo.Plugin.Wallets.PartialPaymentAdmin
{
    public class PartialPaymentAdmin : BasePlugin, IAdminMenuPlugin
    {
        public void ManageSiteMap(SiteMapNode rootNode)
        {
            var menuItem = new SiteMapNode()
            {
                SystemName = "Wallets.PartialPaymentAdmin",
                Title = "Partial Payment Admin",
                ControllerName = "PartialPayment",
                ActionName = "List",
                Visible = true,
                RouteValues = new RouteValueDictionary() {{"area", "admin"}}
            };
            var pluginNode = rootNode.ChildNodes
                .FirstOrDefault(x => x.SystemName == menuItem.SystemName);
            if (pluginNode != null)
            {
                pluginNode.ChildNodes.Add(menuItem);
            }
            else
            {
                rootNode.ChildNodes.Add(menuItem);
            }
        }
    }
}