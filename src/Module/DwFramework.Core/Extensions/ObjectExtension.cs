using System;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;

namespace DwFramework.Core.Extensions
{
    public static class ObjectExtension
    {
        /// <summary>
        /// 类型转换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T ConvertTo<T>(this object obj)
        {
            var value = Convert.ChangeType(obj, typeof(T));
            if (value == null) return default;
            return (T)value;
        }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static string ToJson<T>(this T obj, JsonSerializerOptions options = null) => JsonSerializer.Serialize(obj, options);

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T ToObject<T>(this string json, JsonSerializerOptions options = null) => JsonSerializer.Deserialize<T>(json, options);

        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static byte[] ToBytes(this object obj, Encoding encoding = null)
        {
            if (encoding != null)
            {
                var json = obj.ToJson();
                return encoding.GetBytes(json);
            }
            else
            {
                using var stream = new MemoryStream();
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, obj);
                return stream.GetBuffer();
            }
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bytes"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static T ToObject<T>(this byte[] bytes, Encoding encoding = null)
        {
            if (encoding != null)
            {
                var json = encoding.GetString(bytes);
                return json.ToObject<T>();
            }
            else
            {
                using var stream = new MemoryStream(bytes);
                IFormatter formatter = new BinaryFormatter();
                return (T)formatter.Deserialize(stream);
            }
        }
    }
}
