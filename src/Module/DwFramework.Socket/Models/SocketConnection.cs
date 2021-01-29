using System;
using System.Net.Sockets;
using System.Threading.Tasks;

using DwFramework.Core.Plugins;

namespace DwFramework.Socket
{
    public sealed class SocketConnection
    {
        public string ID { get; init; }
        public System.Net.Sockets.Socket Socket { get; init; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="socket"></param>
        public SocketConnection(System.Net.Sockets.Socket socket)
        {
            ID = MD5.Encode(Guid.NewGuid().ToString());
            socket.SendTimeout = 1000;
            Socket = socket;
        }

        /// <summary>
        /// 检查连接
        /// </summary>
        /// <returns></returns>
        public bool IsAvailable()
        {
            try
            {
                if (!Socket.Poll(-1, SelectMode.SelectRead) || !Socket.Poll(-1, SelectMode.SelectWrite)) return false;
                Socket.Send(Array.Empty<byte>());
                return true;
            }
            catch
            {
                return false;
            }
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
        /// 断开连接
        /// </summary>
        /// <returns></returns>
        public Task CloseAsync()
        {
            return TaskManager.CreateTask(() =>
            {
                Socket.Close();
                Socket.Dispose();
            });
        }
    }
}
