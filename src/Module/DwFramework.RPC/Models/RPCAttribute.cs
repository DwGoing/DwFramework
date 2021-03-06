using System;

namespace DwFramework.RPC
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RPCAttribute : Attribute
    {
        public Type[] ExternalServices { get; init; }

        public RPCAttribute(params Type[] externalServices)
        {
            ExternalServices = externalServices;
        }
    }
}
