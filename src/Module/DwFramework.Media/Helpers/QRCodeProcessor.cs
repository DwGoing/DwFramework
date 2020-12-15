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
        /// <param name="image"></param>
        /// <returns></returns>
        public static string Decode(ImageData image)
        {
            var detector = new QRCodeDetector();
            using var mat = image.ToMat();
            return detector.DetectAndDecode(mat, out var _);
        }

        /// <summary>
        /// 解析二维码
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string Decode(string path)
        {
            using var src = new ImageData(path);
            return Decode(src);
        }

        /// <summary>
        /// 解析二维码
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string Decode(Stream stream)
        {
            using var src = new ImageData(stream);
            return Decode(src);
        }
    }
}
