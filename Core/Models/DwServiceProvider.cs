using Autofac.Extensions.DependencyInjection;

namespace DwFramework.Core
{
    public class DwServiceProvider : AutofacServiceProvider, IDwServiceProvider
    {
        public DwServiceProvider(AutofacServiceProvider provider) : base(provider.LifetimeScope) { }
    }
}
