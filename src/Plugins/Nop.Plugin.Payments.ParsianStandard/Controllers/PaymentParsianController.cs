using System.Net;
using Nop.Plugin.Payments.ParsianStandard.Models;

namespace Nop.Plugin.Payments.ParsianStandard.Controllers
{
    public class PaymentParsianController : BasePaymentController
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly ISettingService _settingService;
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IPermissionService _permissionService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IStoreContext _storeContext;
        private readonly ILogger _logger;
        private readonly IWebHelper _webHelper;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly ParsianPaymentSettings _parsianPaymentSettings;
        private readonly INotificationService _notificationService;

        #endregion

        #region Ctor

        public PaymentParsianController(IWorkContext workContext,
            ISettingService settingService,
            IPaymentService paymentService,
            IOrderService orderService,
            IOrderProcessingService orderProcessingService,
            IPermissionService permissionService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IStoreContext storeContext,
            ILogger logger,
            IWebHelper webHelper,
            ParsianPaymentSettings parsianPaymentSettings,
            ShoppingCartSettings shoppingCartSettings,
            INotificationService notificationService)
        {
            this._workContext = workContext;
            this._settingService = settingService;
            this._paymentService = paymentService;
            this._orderService = orderService;
            this._orderProcessingService = orderProcessingService;
            this._permissionService = permissionService;
            this._genericAttributeService = genericAttributeService;
            this._localizationService = localizationService;
            this._storeContext = storeContext;
            this._logger = logger;
            this._webHelper = webHelper;
            this._parsianPaymentSettings = parsianPaymentSettings;
            this._shoppingCartSettings = shoppingCartSettings;
            this._notificationService = notificationService;
        }

        #endregion

        #region Methods

        
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider
                .ManagePaymentMethods))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var parsianPaymentSettings =
                _settingService.LoadSetting<ParsianPaymentSettings>(storeScope);

            var model = new ConfigurationModel
            {
                Pin = parsianPaymentSettings.PIN
            };

            return View("~/Plugins/Payments.Parsian/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        [AdminAntiForgery]
        [Area(AreaNames.Admin)]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider
                .ManagePaymentMethods))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return Configure();

            var parsianSettings = _settingService
                .LoadSetting<ParsianPaymentSettings>();

            parsianSettings.PIN = model.Pin;

            _settingService.SaveSetting(parsianSettings);
            _settingService.ClearCache();

            _notificationService.SuccessNotification(
                _localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        public ActionResult Pay(string result)
        {
            ViewBag.result = result;
            return View("~/Plugins/Payments.Parsian/Views/Pay.cshtml");
        }

        static void BypassCertificateError()
        {
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender2, certificate, chain, sslPolicyErrors) => true;
        }

        #endregion
    }
}