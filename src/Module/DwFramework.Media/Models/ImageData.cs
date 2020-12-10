using System;
using System.IO;
using SkiaSharp;
using OpenCvSharp;

namespace DwFramework.Media
{
    public sealed class ImageData : IDisposable
    {
        private SKBitmap _skBitmap;

        public int Width => _skBitmap == null ? 0 : _skBitmap.Width;
        public int Height => _skBitmap == null ? 0 : _skBitmap.Height;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="skBitmap"></param>
        public ImageData(SKBitmap skBitmap)
        {
            _skBitmap = skBitmap;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="stream"></param>
        public ImageData(Stream stream)
        {
            if (stream == null || stream.Length <= 0) throw new Exception("stream参数为空");
            using var skStream = new SKManagedStream(stream, true);
            _skBitmap = SKBitmap.Decode(skStream);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="path"></param>
        public ImageData(string path)
        {
            if (string.IsNullOrEmpty(path)) throw new Exception("path为空");
            if (!File.Exists(path)) throw new Exception("图片不存在");
            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            if (stream == null || stream.Length <= 0) throw new Exception("stream参数为空");
            using var skStream = new SKManagedStream(stream, true);
            _skBitmap = SKBitmap.Decode(skStream);
        }

        /// <summary>
        /// 获取字节数组
        /// </summary>
        /// <returns></returns>
        private byte[] ToBytes()
        {
            if (_skBitmap == null) return null;
            var bytes = new byte[Width * Height * 3];
            var byteIndex = 0;
            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    var color = _skBitmap.GetPixel(x, y);
                    bytes[byteIndex + 0] = color.Blue;
                    bytes[byteIndex + 1] = color.Green;
                    bytes[byteIndex + 2] = color.Red;
                    byteIndex += 3;
                }
            }
            return bytes;
        }

        /// <summary>
        /// 获取矩阵
        /// </summary>
        /// <returns></returns>
        public Mat ToMat()
        {
            return new Mat(_skBitmap.Height, _skBitmap.Width, MatType.CV_8UC3, ToBytes());
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            _skBitmap.Dispose();
            _skBitmap = null;
        }
    }
}
