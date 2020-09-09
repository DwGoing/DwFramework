using System;
using System.Text;
using System.Linq;

namespace DwFramework.Extensions.Core
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
        /// <param name="c"></param>
        /// <returns></returns>
        public static int ToBase32Value(this char c)
        {
            var value = (int)c;
            if (value < 91 && value > 64)
            {
                return value - 65;
            }
            if (value < 56 && value > 49)
            {
                return value - 24;
            }
            if (value < 123 && value > 96)
            {
                return value - 97;
            }
            throw new ArgumentException("Character is not a Base32 character.", "c");
        }

        /// <summary>
        /// int转字符
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static char ToBase32Char(this byte b)
        {
            if (b < 26)
            {
                return (char)(b + 65);
            }
            if (b < 32)
            {
                return (char)(b + 24);
            }
            throw new ArgumentException("Byte is not a value Base32 value.", "b");
        }

        /// <summary>
        /// 转Base32字符数组
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] FromBase32String(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                throw new ArgumentNullException("input");
            }

            str = str.TrimEnd('=');
            int byteCount = str.Length * 5 / 8;
            byte[] returnArray = new byte[byteCount];

            byte curByte = 0, bitsRemaining = 8;
            int mask = 0, arrayIndex = 0;

            foreach (char c in str)
            {
                int cValue = ToBase32Value(c);

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
            }

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
            if (bytes == null || bytes.Length == 0)
            {
                throw new ArgumentNullException("input");
            }

            int charCount = (int)Math.Ceiling(bytes.Length / 5d) * 8;
            char[] returnArray = new char[charCount];

            byte nextChar = 0, bitsRemaining = 5;
            int arrayIndex = 0;

            foreach (byte b in bytes)
            {
                nextChar = (byte)(nextChar | (b >> (8 - bitsRemaining)));
                returnArray[arrayIndex++] = ToBase32Char(nextChar);

                if (bitsRemaining < 4)
                {
                    nextChar = (byte)((b >> (3 - bitsRemaining)) & 31);
                    returnArray[arrayIndex++] = ToBase32Char(nextChar);
                    bitsRemaining += 5;
                }

                bitsRemaining -= 3;
                nextChar = (byte)((b << bitsRemaining) & 31);
            }

            if (arrayIndex != charCount)
            {
                returnArray[arrayIndex++] = ToBase32Char(nextChar);
                while (arrayIndex != charCount) returnArray[arrayIndex++] = '=';
            }

            return new string(returnArray);
        }

        /// <summary>
        /// 字节数组转Hex
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string ToHex(this byte[] bytes)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
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
        /// Base64转字符串
        /// </summary>
        /// <param name="base64String"></param>
        /// <returns></returns>
        public static byte[] FromBase64String(this string base64String)
        {
            return Convert.FromBase64String(base64String);
        }

        /// <summary>
        /// Hex转字符串
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static byte[] FromHex(this string hexString)
        {
            var bytes = new byte[hexString.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = (byte)Convert.ToInt32(hexString.Substring(i * 2, 2), 16);
            }
            return bytes;
        }
    }
}
