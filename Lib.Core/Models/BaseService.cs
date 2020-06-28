using System;

namespace DwFramework.Core
{
    public abstract class BaseService
    {
        protected IServiceProvider _provider;
        protected IEnvironment _environment;

        public BaseService(IServiceProvider provider, IEnvironment environment)
        {
            _provider = provider;
            _environment = environment;
        }
    }
}
