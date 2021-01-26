using System;
using System.Text;
using System.Text.Json;
using Mapster;

namespace DwFramework.Core.Extensions
{
    public static class ObjectExtension
    {
        /// <summary>
        /// 类型转换
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object ConvertTo(this object obj, Type type) => obj.Adapt(obj.GetType(), type);

        /// <summary>
        /// 类型转换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T ConvertTo<T>(this object obj) => obj.Adapt<T>();

        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="type"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static string ToJson(this object obj, Type type, JsonSerializerOptions options = null) => JsonSerializer.Serialize(obj, type, options);

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
        /// <param name="json"></param>
        /// <param name="type"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static object ToObject(this string json, Type type, JsonSerializerOptions options = null) => JsonSerializer.Deserialize(json, type, options);

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
            encoding ??= Encoding.UTF8;
            var json = obj.ToJson();
            return encoding.GetBytes(json);
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="type"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static object ToObject(this byte[] bytes, Type type, Encoding encoding = null)
        {
            encoding ??= Encoding.UTF8;
            var json = encoding.GetString(bytes);
            return json.ToObject(type);
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
            encoding ??= Encoding.UTF8;
            var json = encoding.GetString(bytes);
            return json.ToObject<T>();
        }
    }
}
