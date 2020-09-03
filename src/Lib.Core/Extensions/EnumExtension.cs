using System;

namespace DwFramework.Core.Extensions
{
    public static class EnumExtension
    {
        /// <summary>
        /// 是否被定义
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsDefined(this Enum value)
        {
            return Enum.IsDefined(value.GetType(), value);
        }
    }
}
