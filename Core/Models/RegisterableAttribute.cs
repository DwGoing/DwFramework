using System;

namespace DwFramework.Core
{
    public enum Lifetime
    {
        InstancePerDependency,
        Singleton,
        InstancePerLifetimeScope
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class RegisterableAttribute : Attribute
    {
        public Type InterfaceType { get; private set; }
        public Lifetime Lifetime { get; private set; }
        public bool IsAutoActivate { get; private set; }

        public RegisterableAttribute(Type interfaceType = null, Lifetime lifetime = Lifetime.InstancePerDependency, bool isAutoActivate = false)
        {
            InterfaceType = interfaceType;
            Lifetime = lifetime;
            IsAutoActivate = isAutoActivate;
        }
    }
}
