using System;
using System.ComponentModel;

namespace DwFramework.Core.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// 获取枚举的描述
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetDescription(this Enum value)
        {
            var enumType = value.GetType();
            var name = Enum.GetName(enumType, value);
            if (name == null) return null;
            var fieldInfo = enumType.GetField(name);
            if (fieldInfo == null) return null;
            var attr = Attribute.GetCustomAttribute(fieldInfo, typeof(DescriptionAttribute), false) as DescriptionAttribute;
            if (attr == null) return null;
            return attr.Description;
        }
    }
}