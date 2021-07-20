using System;
using System.Text;
using System.Linq;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.International.Converters.PinYinConverter;
using K4os.Compression.LZ4.Streams;

namespace DwFramework.Core
{
    public static class StringExtension
    {
        private static readonly string _characters = "0123456789abcdef";

        /// <summary>
        /// 进制转换
        /// </summary>
        /// <param name="source"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static string ToBinary(this string source, int from, int to)
        {
            if (!new[] { 2, 8, 10, 16 }.Contains(from) || !new[] { 2, 8, 10, 16 }.Contains(to)) throw new Exception("请选择合适的进制:2,8,10,16");
            var index = 0;
            var value = source.Reverse().Sum(item => _characters.IndexOf(item) * Math.Pow(from, index++));
            var builder = new StringBuilder();
            while (value > 0)
            {
                var v = (int)value / to;
                var mod = (int)value - v * to;
                builder.Append(_characters[mod]);
                value = v;
            }
            var chars = builder.ToString().Reverse().ToArray();
            return new string(chars);
        }

        /// <summary>
        /// 字符转int
        /// </summary>
        /// <param name="char"></param>
        /// <returns></returns>
        public static int ToBase32Value(this char @char)
        {
            var value = (int)@char;
            return value switch
            {
                > 49 and < 56 => value - 24,
                > 64 and < 91 => value - 65,
                > 96 and < 123 => value - 97,
                _ => throw new Exception($"非Base32字符:{@char}")
            };
        }

        /// <summary>
        /// int转字符
        /// </summary>
        /// <param name="byte"></param>
        /// <returns></returns>
        public static char ToBase32Char(this byte @byte)
        {
            return @byte switch
            {
                < 26 => (char)(@byte + 65),
                > 26 and < 32 => (char)(@byte + 24),
                _ => throw new Exception($"非Base32字符值:{@byte}")
            };
        }

        /// <summary>
        /// 转Base32字符数组
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] FromBase32String(this string str)
        {
            if (string.IsNullOrEmpty(str)) throw new Exception("参数为空");

            str = str.TrimEnd('=');
            var byteCount = str.Length * 5 / 8;
            var returnArray = new byte[byteCount];

            var curByte = (byte)0;
            var bitsRemaining = (byte)8;
            var arrayIndex = 0;

            str.ForEach(item =>
            {
                var cValue = ToBase32Value(item);
                int mask;
                if (bitsRemaining > 5)
                {
                    mask = cValue << (bitsRemaining - 5);
                    curByte = (byte)(curByte | mask);
                    bitsRemaining -= 5;
                }
                else
                {
                    mask = cValue >> (5 - bitsRemaining);
                    curByte = (byte)(curByte | mask);
                    returnArray[arrayIndex++] = curByte;
                    curByte = (byte)(cValue << (3 + bitsRemaining));
                    bitsRemaining += 3;
                }
            });

            if (arrayIndex != byteCount)
            {
                returnArray[arrayIndex] = curByte;
            }
            return returnArray;
        }

        /// <summary>
        /// 转Base32字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string ToBase32String(this byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) throw new Exception("参数为空");

            var charCount = (int)Math.Ceiling(bytes.Length / 5d) * 8;
            var returnArray = new char[charCount];

            var nextChar = (byte)0;
            var bitsRemaining = (byte)5;
            var arrayIndex = 0;

            bytes.ForEach(item =>
            {
                nextChar = (byte)(nextChar | (item >> (8 - bitsRemaining)));
                returnArray[arrayIndex++] = ToBase32Char(nextChar);

                if (bitsRemaining < 4)
                {
                    nextChar = (byte)((item >> (3 - bitsRemaining)) & 31);
                    returnArray[arrayIndex++] = ToBase32Char(nextChar);
                    bitsRemaining += 5;
                }

                bitsRemaining -= 3;
                nextChar = (byte)((item << bitsRemaining) & 31);
            });

            if (arrayIndex != charCount)
            {
                returnArray[arrayIndex++] = ToBase32Char(nextChar);
                while (arrayIndex != charCount) returnArray[arrayIndex++] = '=';
            }

            return new string(returnArray);
        }

        /// <summary>
        /// 转Base64字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string ToBase64String(this byte[] bytes)
        {
            return Convert.ToBase64String(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Base64转字节数组
        /// </summary>
        /// <param name="base64String"></param>
        /// <returns></returns>
        public static byte[] FromBase64String(this string base64String)
        {
            return Convert.FromBase64String(base64String);
        }

        /// <summary>
        /// 字节数组转Hex
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string ToHex(this byte[] bytes)
        {
            var builder = new StringBuilder();
            for (var i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }

        /// <summary>
        /// Hex转字节数组
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static byte[] FromHex(this string hexString)
        {
            var bytes = new byte[hexString.Length / 2];
            for (var i = 0; i < bytes.Length; i++)
            {
                bytes[i] = (byte)Convert.ToInt32(hexString.Substring(i * 2, 2), 16);
            }
            return bytes;
        }

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
