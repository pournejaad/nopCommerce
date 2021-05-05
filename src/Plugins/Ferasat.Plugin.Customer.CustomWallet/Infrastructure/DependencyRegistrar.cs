using Ferasat.Plugin.Customer.CustomWallet.Services;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;

namespace Ferasat.Plugin.Customer.CustomWallet.Infrastructure
{
    public class DependencyRegistrar:IDependencyRegistrar
    {
        public void Register(IServiceCollection services, ITypeFinder typeFinder, AppSettings appSettings)
        {
            services.AddScoped<IWalletService, WalletService>();
        }

        public int Order => 1;
    }
}