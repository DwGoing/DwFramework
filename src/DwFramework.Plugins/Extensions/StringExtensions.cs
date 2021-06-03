using System;
using System.Linq;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.International.Converters.PinYinConverter;
using K4os.Compression.LZ4.Streams;

namespace DwFramework.Core.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// 是否为中文字符
        /// </summary>
        /// <param name="char"></param>
        /// <returns></returns>
        public static bool IsChinese(this char @char)
        {
            var pattern = @"^[\u4e00-\u9fa5]$";
            if (Regex.IsMatch(@char.ToString(), pattern)) return true;
            return false;
        }

        /// <summary>
        /// 获取中文字符的拼音
        /// </summary>
        /// <param name="@char"></param>
        /// <returns></returns>
        public static string[] GetPinYin(this char @char)
        {
            if (!IsChinese(@char)) return null;
            var decoder = new ChineseChar(@char);
            return decoder.Pinyins;
        }

        /// <summary>
        /// 是否为邮箱地址
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsEmailAddress(this string str)
        {
            var match = new Regex(@"^[a-zA-Z0-9_-]+@[a-zA-Z0-9_-]+(\.[a-zA-Z0-9_-]+)+$").Match(str);
            return match.Success;
        }

        /// <summary>
        /// 压缩
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static async Task<byte[]> Compress(this byte[] bytes, CompressType type)
        {
            using var sourceStream = new MemoryStream(bytes);
            using var targetStream = new MemoryStream();
            using dynamic compressionStream = type switch
            {
                CompressType.Brotli => new BrotliStream(targetStream, CompressionMode.Compress),
                CompressType.GZip => new GZipStream(targetStream, CompressionMode.Compress),
                CompressType.LZ4 => LZ4Stream.Encode(targetStream),
                _ => throw new Exception("未知压缩类型")
            };
            await sourceStream.CopyToAsync(compressionStream);
            compressionStream.Close();
            return targetStream.ToArray();
        }

        /// <summary>
        /// 解压
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static async Task<byte[]> Decompress(this byte[] bytes, CompressType type)
        {
            using var soureStream = new MemoryStream(bytes);
            using var targetStream = new MemoryStream();
            using dynamic decompressionStream = type switch
            {
                CompressType.Brotli => new BrotliStream(soureStream, CompressionMode.Decompress),
                CompressType.GZip => new GZipStream(soureStream, CompressionMode.Decompress),
                CompressType.LZ4 => LZ4Stream.Decode(soureStream),
                _ => throw new Exception("未知压缩类型")
            };
            await decompressionStream.CopyToAsync(targetStream);
            return targetStream.ToArray();
        }

        /// <summary>
        /// 计算相似度
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static double ComputeSimilarity(this string source, string target)
        {
            var len1 = source.Length;
            var len2 = target.Length;
            int[,] diff = new int[len1 + 1, len2 + 1];
            diff[0, 0] = 0;
            for (var i = 1; i <= len1; i++) diff[i, 0] = i;
            for (var i = 1; i <= len2; i++) diff[0, i] = i;
            var ch1 = source.ToCharArray();
            var ch2 = target.ToCharArray();
            for (var i = 1; i <= len1; i++)
            {
                for (var j = 1; j <= len2; j++)
                {
                    var min = new int[] { diff[i - 1, j - 1], diff[i - 1, j], diff[i, j - 1] }.Min();
                    diff[i, j] = ch1[i - 1] == ch2[j - 1] ? min : min + 1;
                }
            }
            return 1 - (double)diff[len1, len2] / Math.Max(len1, len2);
        }

        /// <summary>
        /// 是否为全角字符
        /// </summary>
        /// <param name="char"></param>
        /// <returns></returns>
        public static bool IsSBC(this char @char)
        {
            var pattern = @"^[\uFF00-\uFFFF]$";
            if (Regex.IsMatch(@char.ToString(), pattern)) return true;
            return false;
        }

        /// <summary>
        /// 获取字符串长度
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int GetLength(this string str)
        {
            var len = 0;
            str.ForEach(item =>
            {
                if (item.IsChinese() || item.IsSBC()) len += 2;
                else len++;
            });
            return len;
        }
    }
}
