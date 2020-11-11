using System;
using System.Collections.Generic;
using System.Data;

namespace DwFramework.Core.Extensions
{
    public static class DataTableExtension
    {
        /// <summary>
        /// DataTable转实体数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataTable"></param>
        /// <param name="propertyFunc"></param>
        /// <param name="convertFunc"></param>
        /// <returns></returns>
        public static T[] ToArray<T>(this DataTable dataTable, Func<string, string> propertyFunc = null, Dictionary<Type, Func<object, object>> convertFunc = null)
        {
            var arr = new T[dataTable.Rows.Count];
            var properties = typeof(T).GetProperties();
            for (var i = 0; i < arr.Length; i++)
            {
                arr[i] = Activator.CreateInstance<T>();
                var row = dataTable.Rows[i];
                properties.ForEach(property =>
                {
                    if (!dataTable.Columns.Contains(propertyFunc != null ? propertyFunc(property.Name) : property.Name)) return;
                    var srcData = row[property.Name];
                    var srcType = srcData.GetType();
                    property.SetValue(arr[i], convertFunc != null && convertFunc.ContainsKey(srcType) ? convertFunc[srcType](srcData) : srcData);
                });
            }
            return arr;
        }
    }
}
