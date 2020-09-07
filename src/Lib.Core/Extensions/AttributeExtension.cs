using System;
using System.Reflection;

namespace DwFramework.Core.Extensions
{
    public static class AttributeExtension
    {
        /// <summary>
        /// 获取对象的描述
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static DescriptionAttribute GetDescription(this MemberInfo info)
        {
            return info.GetCustomAttribute<DescriptionAttribute>();
        }
    }
}
