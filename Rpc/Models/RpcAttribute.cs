using System;

namespace DwFramework.Rpc
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RpcAttribute : Attribute
    {
        public string CallName { get; private set; }

        public RpcAttribute(string callName = null)
        {
            CallName = callName;
        }
    }
}
