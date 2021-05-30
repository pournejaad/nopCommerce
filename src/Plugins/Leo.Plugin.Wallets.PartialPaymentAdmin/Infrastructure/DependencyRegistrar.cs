using Autofac;
using Leo.Plugin.Wallets.PartialPaymentAdmin.Factories;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;

namespace Leo.Plugin.Wallets.PartialPaymentAdmin.Infrastructure
{
    public class DependencyRegistrar :IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<PartialPaymentModelFactory>().As<IPartialPaymentModelFactory>()
                .InstancePerLifetimeScope();
        }
        

        public int Order => 101;
    }
}