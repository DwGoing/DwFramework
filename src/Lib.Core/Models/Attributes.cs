using System;

namespace DwFramework.Core
{
    /// <summary>
    /// 可注册的对象
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
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

    /// <summary>
    /// 对象的描述
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
    public class DescriptionAttribute : Attribute
    {
        public readonly string Name;
        public readonly string Details;

        public DescriptionAttribute(string name, string details)
        {
            Name = name;
            Details = details;
        }
    }
}
