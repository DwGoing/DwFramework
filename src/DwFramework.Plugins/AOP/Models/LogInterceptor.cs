using System;
using Microsoft.Extensions.DependencyInjection;
using Autofac;
using Autofac.Extras.DynamicProxy;
using Castle.DynamicProxy;
using NLog;
using DwFramework.Core;

namespace DwFramework.Plugins.AOP
{
    public sealed class LogInterceptor : IInterceptor
    {
        private readonly LogLevel _logLevel = LogLevel.Off;

        public LogInterceptor(LogLevel logLevel)
        {
            _logLevel = logLevel;
        }

        public void Intercept(IInvocation invocation)
        {
            var logger = LogManager.GetLogger($"{invocation.TargetType.Name}InvokeLog");
            invocation.Proceed();
            logger?.Log(_logLevel,
                "\n========================================\n"
                + $"Method:\t{invocation.Method}\n"
                + $"Args:\t{string.Join('|', invocation.Arguments)}\n"
                + $"Return:\t{invocation.ReturnValue}\n"
                + "========================================"
            );
        }
    }
}