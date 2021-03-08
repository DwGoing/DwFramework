using System;

namespace DwFramework.RPC
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RPCAttribute : Attribute
    {
        public Type[] ExternalServices { get; set; }

        public RPCAttribute(params Type[] externalServices)
        {
            ExternalServices = externalServices;
        }
    }
}
