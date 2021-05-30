namespace Nop.Plugin.Payments.ParsianStandard.Components
{
    [ViewComponent(Name = "PaymentParsian")]
    public class ParsianViewComponent : NopViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View("~/Plugins/Payments.Parsian/Views/PaymentInfo.cshtml");
        }
    }
}
