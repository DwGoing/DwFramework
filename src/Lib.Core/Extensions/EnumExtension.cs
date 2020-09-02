using System;

namespace DwFramework.Core.Extensions
{
    public static class EnumExtension
    {
        /// <summary>
        /// 是否被定义
        /// </summary>
        /// <param name="enum"></param>
        /// <returns></returns>
        public static bool IsDefined(this Enum @enum)
        {
            return Enum.IsDefined(@enum.GetType(), @enum);
        }
    }
}
