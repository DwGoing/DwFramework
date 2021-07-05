using System;
using System.Collections.Generic;
using System.Data;

namespace DwFramework.Core
{
    public static class DataTableExtension
    {
        /// <summary>
        /// DataTable转实体数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataTable"></param>
        /// <param name="convertFunc"></param>
        /// <param name="properyFunc"></param>
        /// <returns></returns>
        public static T[] ToArray<T>(this DataTable dataTable, Dictionary<Type, Func<object, object>> convertFunc = null, Dictionary<string, Func<object, object>> propertyFunc = null)
        {
            var arr = new T[dataTable.Rows.Count];
            var properties = typeof(T).GetProperties();
            for (var i = 0; i < arr.Length; i++)
            {
                arr[i] = Activator.CreateInstance<T>();
                var row = dataTable.Rows[i];
                properties.ForEach(property =>
                {
                    var srcData = row[property.Name];
                    // 自定义类型转换
                    var srcType = srcData.GetType();
                    srcData = convertFunc != null && convertFunc.ContainsKey(srcType) ? convertFunc[srcType](srcData) : srcData;
                    // 自定义字段转换
                    srcData = propertyFunc != null && propertyFunc.ContainsKey(property.Name) ? propertyFunc[property.Name](srcData) : srcData;
                    property.SetValue(arr[i], srcData);
                });
            }
            return arr;
        }
    }
}
