using Leo.Services.PartialPayments;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;

namespace Leo.Services.Infrastructure
{
    public class LeoStartup:INopStartup
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IPartialPaymentService, PartialPaymentService>();
        }

        public void Configure(IApplicationBuilder application)
        {
        }

        public int Order => 102;
    }
}