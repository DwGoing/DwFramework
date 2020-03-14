using System;

using Castle.DynamicProxy;

namespace DwFramework.Core.Plugins
{
    public class CallInfo
    {
        public readonly IInvocation Invocation;

        public CallInfo(IInvocation invocation)
        {
            Invocation = invocation;
        }
    }

    public abstract class BaseInterceptor : IInterceptor
    {
        public abstract void OnCall(CallInfo info);

        public void Intercept(IInvocation invocation)
        {
            OnCall(new CallInfo(invocation));
        }
    }
}
