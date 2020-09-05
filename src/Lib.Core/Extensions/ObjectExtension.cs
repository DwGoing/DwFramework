using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using Newtonsoft.Json;

namespace DwFramework.Core.Extensions
{
    public static class ObjectExtension
    {
        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="isIndented"></param>
        /// <returns></returns>
        public static string ToJson(this object obj, bool isIndented = false)
        {
            if (isIndented) return JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });
            else return JsonConvert.SerializeObject(obj);
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <param name="isIndented"></param>
        /// <returns></returns>
        public static T ToObject<T>(this string json, bool isIndented = false)
        {
            if (isIndented) return JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });
            else return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] ToBytes(this object obj)
        {
            using var stream = new MemoryStream();
            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, obj);
            return stream.GetBuffer();
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static T ToObject<T>(this byte[] bytes)
        {
            using var stream = new MemoryStream(bytes);
            IFormatter formatter = new BinaryFormatter();
            return (T)formatter.Deserialize(stream);
        }
    }
}
