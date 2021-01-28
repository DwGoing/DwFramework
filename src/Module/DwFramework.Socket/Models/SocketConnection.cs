using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using DwFramework.Core.Plugins;

namespace DwFramework.Socket
{
    public sealed class SocketConnection : IDisposable
    {
        public bool IsClose { get; private set; } = false;
        public string ID { get; init; }
        public System.Net.Sockets.Socket Socket { get; init; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="socket"></param>
        public SocketConnection(System.Net.Sockets.Socket socket)
        {
            ID = MD5.Encode(Guid.NewGuid().ToString());
            Socket = socket;
        }

        /// <summary>
        /// 检查连接
        /// </summary>
        /// <returns></returns>
        public bool CheckConnection()
        {
            if (IsClose) return false;
            return !Socket.Poll(1000, SelectMode.SelectRead);
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public Task SendAsync(byte[] buffer)
        {
            return TaskManager.CreateTask(() => Socket.Send(new ArraySegment<byte>(buffer)));
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public Task SendAsync(string msg)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(msg);
            return SendAsync(buffer);
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <returns></returns>
        public Task CloseAsync()
        {
            IsClose = true;
            return TaskManager.CreateTask(() => Socket.Close());
        }

        /// <summary>
        /// 释放连接
        /// </summary>
        public void Dispose()
        {
            Socket.Dispose();
        }
    }
}
