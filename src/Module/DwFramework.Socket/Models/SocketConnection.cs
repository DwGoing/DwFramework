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
        public bool PreClose { get; private set; } = false;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="socket"></param>
        public SocketConnection(System.Net.Sockets.Socket socket)
        {
            ID = MD5.Encode(Guid.NewGuid().ToString());
            Socket = socket;
            Socket.EnableKeepAlive(3000, 500);
            Console.WriteLine(Socket.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive));
        }

        /// <summary>
        /// 检查连接
        /// </summary>
        /// <returns></returns>
        public bool IsAvailable()
        {
            try
            {
                return Socket.Poll(5000, SelectMode.SelectRead) || Socket.Poll(5000, SelectMode.SelectWrite);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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
        /// <param name="preClose"></param>
        /// <returns></returns>
        public Task CloseAsync(bool preClose = false)
        {
            if (preClose && !PreClose)
            {
                PreClose = true;
                return Task.CompletedTask;
            }
            return TaskManager.CreateTask(() =>
            {
                try
                {
                    Socket.Shutdown(SocketShutdown.Both);
                }
                finally
                {
                    Socket.Close();
                }
            });
        }
    }
}
