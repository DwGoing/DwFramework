using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Xml;
using System.Xml.Serialization;

namespace DwFramework.Core
{
    public static class ObjectExtension
    {
        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="type"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static string ToJson(this object obj, Type type, JsonSerializerOptions options = null)
        {
            return JsonSerializer.Serialize(obj, type, options);
        }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static string ToJson<T>(this T obj, JsonSerializerOptions options = null)
        {
            return JsonSerializer.Serialize(obj, options);
        }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="type"></param>
        /// <param name="encoding"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static byte[] ToJsonBytes(this object obj, Type type, Encoding encoding = null, JsonSerializerOptions options = null)
        {
            encoding ??= Encoding.UTF8;
            var json = obj.ToJson(type, options);
            return encoding.GetBytes(json);
        }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="encoding"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static byte[] ToJsonBytes<T>(this T obj, Encoding encoding = null, JsonSerializerOptions options = null)
        {
            encoding ??= Encoding.UTF8;
            var json = obj.ToJson(typeof(T), options);
            return encoding.GetBytes(json);
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="json"></param>
        /// <param name="type"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static object FromJson(this string json, Type type, JsonSerializerOptions options = null)
        {
            return JsonSerializer.Deserialize(json, type, options);
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static T FromJson<T>(this string json, JsonSerializerOptions options = null)
        {
            return JsonSerializer.Deserialize<T>(json, options);
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="type"></param>
        /// <param name="encoding"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static object FromJsonBytes(this byte[] bytes, Type type, Encoding encoding = null, JsonSerializerOptions options = null)
        {
            encoding ??= Encoding.UTF8;
            var json = encoding.GetString(bytes);
            return json.FromJson(type, options);
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bytes"></param>
        /// <param name="encoding"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static T FromJsonBytes<T>(this byte[] bytes, Encoding encoding = null, JsonSerializerOptions options = null)
        {
            encoding ??= Encoding.UTF8;
            var json = encoding.GetString(bytes);
            return json.FromJson<T>(options);
        }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="type"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string ToXml(this object obj, Type type, Encoding encoding = null)
        {
            encoding ??= Encoding.UTF8;
            return encoding.GetString(obj.ToXmlBytes(type, encoding));
        }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string ToXml<T>(this T obj, Encoding encoding = null)
        {
            return ToXml(obj, typeof(T), encoding);
        }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="type"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static byte[] ToXmlBytes(this object obj, Type type, Encoding encoding = null)
        {
            encoding ??= Encoding.UTF8;
            using var output = new MemoryStream();
            using var writer = new XmlTextWriter(output, encoding);
            var serializer = new XmlSerializer(type);
            serializer.Serialize(writer, obj);
            return output.ToArray();
        }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static byte[] ToXmlBytes<T>(this T obj, Encoding encoding = null)
        {
            return obj.ToXmlBytes(typeof(T), encoding);
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="type"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static object FromXml(this string xml, Type type, Encoding encoding = null)
        {
            encoding ??= Encoding.UTF8;
            return encoding.GetBytes(xml).FromXmlBytes(type);
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static T FromXml<T>(this string xml, Encoding encoding = null) where T : class
        {
            return xml.FromXml(typeof(T), encoding) as T;
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object FromXmlBytes(this byte[] bytes, Type type)
        {
            using var input = new MemoryStream(bytes);
            var serializer = new XmlSerializer(type);
            return serializer.Deserialize(input);
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static T FromXmlBytes<T>(this byte[] bytes) where T : class
        {
            return bytes.FromXmlBytes(typeof(T)) as T;
        }
    }
}
