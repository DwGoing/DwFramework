using System;

namespace DwFramework.Web.Rpc
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RPCAttribute : Attribute
    {
        public RPCAttribute() { }
    }
}
