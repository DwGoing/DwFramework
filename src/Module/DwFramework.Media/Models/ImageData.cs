using System;
using System.IO;
using System.Drawing;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace DwFramework.Media
{
    public sealed class ImageData : IDisposable
    {
        private Stream _stream;

        public int Width => GetWidth();
        public int Height => GetHeight();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="stream"></param>
        public ImageData(Stream stream)
        {
            if (stream == null || stream.Length <= 0) throw new Exception("stream参数为空");
            _stream = stream;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="mat"></param>
        public ImageData(Mat mat)
        {
            if (mat == null || mat.Empty()) throw new Exception("mat参数为空");
            _stream = mat.ToMemoryStream();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="path"></param>
        public ImageData(string path)
        {
            if (string.IsNullOrEmpty(path)) throw new Exception("path为空");
            if (!File.Exists(path)) throw new Exception("图片不存在");
            _stream = new FileStream(path, FileMode.Open, FileAccess.Read);
        }

        /// <summary>
        /// 获取流
        /// </summary>
        /// <returns></returns>
        public Stream ToStream()
        {
            if (_stream == null || _stream.Length <= 0) throw new Exception("数据长度为0");
            return _stream;
        }

        /// <summary>
        /// 获取矩阵
        /// </summary>
        /// <returns></returns>
        public Mat ToMat()
        {
            if (_stream == null || _stream.Length <= 0) throw new Exception("数据长度为0");
            return Mat.FromStream(_stream, ImreadModes.Unchanged);
        }

        /// <summary>
        /// 获取字节数组
        /// </summary>
        /// <returns></returns>
        public byte[] ToBytes()
        {
            if (_stream == null || _stream.Length <= 0) throw new Exception("数据长度为0");
            using var mat = ToMat();
            return mat.ToBytes();
        }

        /// <summary>
        /// 获取Bitmap
        /// </summary>
        /// <returns></returns>
        public Bitmap ToBitmap()
        {
            if (_stream == null || _stream.Length <= 0) throw new Exception("数据长度为0");
            using var mat = ToMat();
            return mat.ToBitmap();
        }

        /// <summary>
        /// 获取宽度
        /// </summary>
        /// <returns></returns>
        public int GetWidth()
        {
            using var mat = ToMat();
            return mat.Width;
        }

        /// <summary>
        /// 获取高度
        /// </summary>
        /// <returns></returns>
        public int GetHeight()
        {
            using var mat = ToMat();
            return mat.Height;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            _stream.Dispose();
            _stream = null;
        }
    }
}
