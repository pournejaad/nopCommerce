using Leo.Plugin.Payments.CustomerWallet.Factories;
using Leo.Plugin.Payments.CustomerWallet.Services;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;

namespace Leo.Plugin.Payments.CustomerWallet.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(IServiceCollection services, ITypeFinder typeFinder, AppSettings appSettings)
        {
            services.AddScoped<IPartialPaymentModelFactory, PartialPaymentModelFactory>();
            services.AddScoped<IPartialPaymentService, PartialPaymentService>();
        }

        public int Order => 1;
    }
}