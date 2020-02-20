using System;

namespace DwFramework.Core.Extensions
{
    public static class StringExtension
    {
        /// <summary>
        /// 字符转int
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static int ToValue(this char c)
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
        public static char ToChar(this byte b)
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
        /// 转Base32zijie数组
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] ToBase32Bytes(this string str)
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
                int cValue = ToValue(c);

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
                returnArray[arrayIndex++] = ToChar(nextChar);

                if (bitsRemaining < 4)
                {
                    nextChar = (byte)((b >> (3 - bitsRemaining)) & 31);
                    returnArray[arrayIndex++] = ToChar(nextChar);
                    bitsRemaining += 5;
                }

                bitsRemaining -= 3;
                nextChar = (byte)((b << bitsRemaining) & 31);
            }

            if (arrayIndex != charCount)
            {
                returnArray[arrayIndex++] = ToChar(nextChar);
                while (arrayIndex != charCount) returnArray[arrayIndex++] = '=';
            }

            return new string(returnArray);
        }
    }
}
