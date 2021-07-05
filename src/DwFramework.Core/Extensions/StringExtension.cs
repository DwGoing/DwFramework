using System;
using System.Text;
using System.Linq;

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
    }
}
