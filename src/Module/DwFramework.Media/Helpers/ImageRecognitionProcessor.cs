using System;
using System.IO;
using OpenCvSharp;

namespace DwFramework.Media
{
    public static class ImageRecognitionProcessor
    {
        private static CascadeClassifier _classifier;

        public static void Init(string xmlPath)
        {
            if (string.IsNullOrEmpty(xmlPath)) throw new Exception("xmlPath参数为空");
            if (!File.Exists(xmlPath)) throw new Exception("文件不存在");
            _classifier = new CascadeClassifier(xmlPath);
        }

        public static void Recognize()
        {
        }
    }
}
