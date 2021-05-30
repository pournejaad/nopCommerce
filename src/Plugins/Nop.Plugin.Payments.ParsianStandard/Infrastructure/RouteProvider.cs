namespace Nop.Plugin.Payments.ParsianStandard.Infrastructure
{
    public partial class RouteProvider : IRouteProvider
    {
        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="routeBuilder">Route builder</param>
        public void RegisterRoutes(IRouteBuilder routeBuilder)
        {
            routeBuilder.MapRoute("Payments.Parsian.ResultHandler",
                "Plugins/PaymentParsian/ResultHandler",
                new {controller = "PaymentParsian", action = "ResultHandler"});
        }

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority => -1;
    }
}