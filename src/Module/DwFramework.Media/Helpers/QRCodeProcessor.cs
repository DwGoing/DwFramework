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
        /// <param name="path"></param>
        /// <returns></returns>
        public static string Decode(string path)
        {
            var src = new ImageData(path);
            var detector = new QRCodeDetector();
            return detector.DetectAndDecode(src.ToMat(), out var _);
        }

        /// <summary>
        /// 解析二维码
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string Decode(Stream stream)
        {
            var src = new ImageData(stream);
            var detector = new QRCodeDetector();
            return detector.DetectAndDecode(src.ToMat(), out var _);
        }
    }
}
