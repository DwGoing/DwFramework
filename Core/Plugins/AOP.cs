using System;
using System.Reflection;

using Castle.DynamicProxy;

namespace DwFramework.Core.Plugins
{
    public class CallInfo
    {
        private readonly IInvocation _invocation;
        public object[] Arguments { get { return _invocation.Arguments; } }
        public Type[] GenericArguments { get { return _invocation.GenericArguments; } }
        public object InvocationTarget { get { return _invocation.InvocationTarget; } }
        public MethodInfo Method { get { return _invocation.Method; } }
        public MethodInfo MethodInvocationTarget { get { return _invocation.MethodInvocationTarget; } }
        public object Proxy { get { return _invocation.Proxy; } }
        public object ReturnValue { get { return _invocation.ReturnValue; } }
        public Type TargetType { get { return _invocation.TargetType; } }

        public CallInfo(IInvocation invocation)
        {
            _invocation = invocation;
        }
    }

    public abstract class BaseInterceptor : IInterceptor
    {
        public abstract void OnCalling(CallInfo info);
        public abstract void OnCalled(CallInfo info);

        public void Intercept(IInvocation invocation)
        {
            OnCalling(new CallInfo(invocation));
            //在被拦截的方法执行完毕后 继续执行
            invocation.Proceed();
            OnCalled(new CallInfo(invocation));
        }
    }
}
