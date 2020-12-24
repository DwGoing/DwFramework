using System;
using System.IO;
using OpenCvSharp;

namespace DwFramework.Media
{
    public static class QRCodeProcessor
    {
        /// <summary>
        /// 解析二维码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Decode(Mat input)
        {
            var detector = new QRCodeDetector();
            return detector.DetectAndDecode(input, out var _);
        }

        /// <summary>
        /// 解析二维码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Decode(string input)
        {
            using var src = new Mat(input);
            return Decode(src);
        }

        /// <summary>
        /// 解析二维码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Decode(Stream input)
        {
            using var src = Mat.FromStream(input, ImreadModes.Unchanged);
            return Decode(src);
        }
    }
}
