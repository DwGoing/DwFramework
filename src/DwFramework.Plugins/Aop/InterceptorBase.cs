using Castle.DynamicProxy;

namespace DwFramework.Core.Plugins
{
    public abstract class InterceptorBase : IInterceptor
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
