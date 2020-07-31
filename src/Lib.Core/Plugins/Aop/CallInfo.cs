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

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="invocation"></param>
        public CallInfo(IInvocation invocation)
        {
            _invocation = invocation;
        }
    }
}
