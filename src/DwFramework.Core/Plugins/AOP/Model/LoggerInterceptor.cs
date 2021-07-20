using System;
using Castle.DynamicProxy;
using NLog;

namespace DwFramework.Core.AOP
{
    public sealed class LoggerInterceptor : IInterceptor
    {
        private readonly Func<IInvocation, (string LoggerName, LogLevel Level, string Context)> _invocationHandler;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="invocationHandler"></param>
        public LoggerInterceptor(Func<IInvocation, (string LoggerName, LogLevel Level, string Context)> invocationHandler)
        {
            _invocationHandler = invocationHandler;
        }

        /// <summary>
        /// 拦截调用
        /// </summary>
        /// <param name="invocation"></param>
        public void Intercept(IInvocation invocation)
        {
            invocation.Proceed();
            var result = _invocationHandler(invocation);
            var logger = LogManager.GetLogger(result.LoggerName);
            if (logger == null) return;
            logger?.Log(result.Level, result.Context);
        }
    }
}