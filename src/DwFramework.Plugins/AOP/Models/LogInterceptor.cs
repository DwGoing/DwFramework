using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Autofac;
using Autofac.Extras.DynamicProxy;
using Castle.DynamicProxy;
using DwFramework.Core;

namespace DwFramework.Plugins.AOP
{
    public sealed class LogInterceptor : IInterceptor
    {
        private readonly LogLevel _logLevel = LogLevel.None;

        public LogInterceptor(LogLevel logLevel)
        {
            _logLevel = logLevel;
        }

        public void Intercept(IInvocation invocation)
        {
            var logger = ServiceHost.ServiceProvider.GetService<ILogger<>>();
            invocation.Proceed();
            Console.WriteLine("OnCalled");
        }
    }
}