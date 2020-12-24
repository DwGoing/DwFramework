using System;
using System.Threading;
using System.Threading.Tasks;
using OpenCvSharp;

using DwFramework.Core.Plugins;

namespace DwFramework.Media
{
    public static class CameraManager
    {
        private static VideoCapture _videoCapture;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="deviceId"></param>
        public static void Init(int deviceId)
        {
            if (_videoCapture != null) return;
            _videoCapture = new VideoCapture(deviceId);
            if (_videoCapture == null || !_videoCapture.IsOpened())
                throw new Exception("初始化失败");
        }

        /// <summary>
        /// 读取帧
        /// </summary>
        /// <returns></returns>
        private static Mat ReadFrame()
        {
            if (_videoCapture == null || !_videoCapture.IsOpened()) throw new Exception("未初始化");
            var frame = new Mat();
            _videoCapture.Read(frame);
            return frame;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public static void Dispose()
        {
            _videoCapture?.Dispose();
        }

        private static readonly object _lock = new object();
        private static Task _task;
        private static CancellationTokenSource _cancellationToken;
        private static Mat _frame;

        /// <summary>
        /// 拍摄
        /// </summary>
        /// <param name="action"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="frameRate"></param>
        public static void Take(Action<Mat> action, int width = 960, int height = 640, int frameRate = 30)
        {
            if (_task != null) throw new Exception("正在运行");
            lock (_lock)
            {
                _task = TaskManager.CreateTask(cancellationToken =>
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        _frame = ReadFrame();
                        _frame = _frame.Resize(new Size(width, height));
                        action?.Invoke(_frame);
                        Thread.Sleep(1000 / frameRate);
                    }
                }, out _cancellationToken);
            }
        }

        /// <summary>
        /// 停止拍摄
        /// </summary>
        public static void Stop()
        {
            if (_task == null) return;
            _cancellationToken.Cancel();
            _cancellationToken.Dispose();
            _cancellationToken = null;
            _task = null;
            _frame.Dispose();
            _frame = null;
        }

        /// <summary>
        /// 停止拍摄并返回最后一帧
        /// </summary>
        /// <returns></returns>
        public static Mat StopReturnFrame()
        {
            var result = _frame?.Clone();
            Stop();
            return result;
        }
    }
}
