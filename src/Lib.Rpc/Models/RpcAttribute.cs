using System;

namespace DwFramework.Rpc
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RpcAttribute : Attribute
    {
        public Type ImplementType { get; private set; }

        public RpcAttribute(Type implementType)
        {
            ImplementType = implementType;
        }
    }
}
