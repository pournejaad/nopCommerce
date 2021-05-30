using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Payments.ParsianStandard.Models;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Payments;
using Nop.Services.Plugins;

namespace Nop.Plugin.Payments.ParsianStandard
{
    /// <summary>
    /// Parsian payment processor
    /// </summary>
    public class ParsianPaymentProcessor : BasePlugin, IPaymentMethod
    {
        private readonly ICustomerService _customerService;

        private readonly ParsianPaymentSettings
            _parsianPaymentSettings;

        private readonly IWebHelper _webHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISettingService _settingService;

        public ParsianPaymentProcessor(ICustomerService customerService,
            ParsianPaymentSettings parsianPaymentSettings, IWebHelper webHelper,
            IHttpContextAccessor httpContextAccessor, ISettingService settingService)
        {
            _customerService = customerService;
            _parsianPaymentSettings = parsianPaymentSettings;
            _webHelper = webHelper;
            _httpContextAccessor = httpContextAccessor;
            _settingService = settingService;
        }

        public Task<ProcessPaymentResult> ProcessPaymentAsync(
            ProcessPaymentRequest processPaymentRequest)
        {
            return Task.FromResult(new ProcessPaymentResult());
        }

        public async Task PostProcessPaymentAsync(
            PostProcessPaymentRequest postProcessPaymentRequest)
        {
            var order = postProcessPaymentRequest.Order;
            var customer = await _customerService.GetCustomerByIdAsync(order
                .CustomerId);

            var requestData = new ClientSalePaymentRequestData
            {
                LoginAccount = _parsianPaymentSettings.PIN,
                Amount = order.OrderTotal,
                OrderId = order.Id,
                CallBackUrl =
                    $"{_webHelper.GetStoreLocation()}Plugins/ParsianPayment/ResultHandler"
            };
            var httpWebRequest = (HttpWebRequest) WebRequest.Create(
                string.Format(SalePaymentRequest.Url));
            httpWebRequest.Method = WebRequestMethods.Http.Post;
            httpWebRequest.ContentType = "application/json";
            using (var streamWriter =
                new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                await streamWriter.WriteAsync(
                    JsonConvert.SerializeObject(requestData));
                await streamWriter.FlushAsync();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse) httpWebRequest.GetResponse();
            var result = string.Empty;
            using (var streamReader =
                new StreamReader(httpResponse.GetResponseStream()))
            {
                result = await streamReader.ReadToEndAsync();
            }

            var salePaymentResponseData =
                JsonConvert.DeserializeObject<ClientSalePaymentResponseData>(result);
            if (salePaymentResponseData.Status == 0 &&
                salePaymentResponseData.Token > 0)
            {
                var paymentURL =
                    string.Format(PaymentUrl.FormattedUrlWithToken,
                        salePaymentResponseData.Token);
                _httpContextAccessor.HttpContext.Response.Redirect(paymentURL);
            }
            else
            {
                throw new Exception("Payment ended unsuccessfully!!");
            }
        }

        public Task<bool> HidePaymentMethodAsync(IList<ShoppingCartItem> cart)
        {
            return Task.FromResult(string.IsNullOrWhiteSpace(_parsianPaymentSettings
                .PIN));
        }

        public Task<decimal> GetAdditionalHandlingFeeAsync(IList<ShoppingCartItem>
            cart)
        {
            return Task.FromResult(decimal.Zero);
        }

        public Task<CapturePaymentResult> CaptureAsync(
            CapturePaymentRequest capturePaymentRequest)
        {
            return Task.FromResult(new CapturePaymentResult()
            {
                Errors = new[] {"Capture not supported."}
            });
        }

        public Task<RefundPaymentResult> RefundAsync(RefundPaymentRequest 
        refundPaymentRequest)
        {
            return Task.FromResult(new RefundPaymentResult()
            {
                Errors = new[] {"Refund not supported"}
            });
        }

        public Task<VoidPaymentResult> VoidAsync(VoidPaymentRequest voidPaymentRequest)
        {
            return Task.FromResult(new VoidPaymentResult()
            {
                Errors = new[] {"Void method not supported."}
            });
        }

        public  Task<ProcessPaymentResult> ProcessRecurringPaymentAsync(
            ProcessPaymentRequest processPaymentRequest)
        {
            return Task.FromResult(new ProcessPaymentResult()
            {
                Errors = new[] {"Recurring payment not supported baby"}
            });
        }

        public Task<CancelRecurringPaymentResult> CancelRecurringPaymentAsync(
            CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            return Task.FromResult(new CancelRecurringPaymentResult()
            {
                Errors = new[] {"Recurring payment not supported baby."}
            });
        }

        public Task<bool> CanRePostProcessPaymentAsync(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            return Task.FromResult(!((DateTime.UtcNow - order.CreatedOnUtc)
            .TotalSeconds < 5));
        }

        public Task<IList<string>> ValidatePaymentFormAsync(IFormCollection form)
        {
            return new Task<IList<string>>();
        }

        public ProcessPaymentRequest GetPaymentInfo(IFormCollection form)
        {
            return new ProcessPaymentRequest();
        }

        public string GetPublicViewComponentName()
        {
            return "PaymentParsian";
        }

        public override async Task InstallAsync()
        {
            await _settingService.SaveSettingAsync(new ParsianPaymentSettings()
            {
                PIN = ""
            });
            await base.InstallAsync();
        }

        public override async Task UninstallAsync()
        {
            await _settingService.DeleteSettingAsync<ParsianPaymentSettings>();
            await base.UninstallAsync();
        }

        public bool SupportCapture => false;
        public bool SupportPartiallyRefund => false;
        public bool SupportRefund => false;
        public bool SupportVoid => false;

        public RecurringPaymentType RecurringPaymentType =>
            RecurringPaymentType.NotSupported;

        public PaymentMethodType PaymentMethodType => PaymentMethodType.Redirection;
        public bool SkipPaymentInfo => false;

        public string PaymentMethodDescription =>
            "Parsian Payment Gateway";
    }
}