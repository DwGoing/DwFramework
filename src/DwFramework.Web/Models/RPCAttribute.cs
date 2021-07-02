using System;

namespace DwFramework.Web
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RPCAttribute : Attribute
    {
        public RPCAttribute() { }
    }
}
