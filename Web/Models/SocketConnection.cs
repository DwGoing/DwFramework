using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using DwFramework.Core.Plugins;
using DwFramework.Core.Helper;

namespace DwFramework.Web
{
    public class SocketConnection
    {
        public readonly string ID;
        public readonly Socket Socket;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="socket"></param>
        public SocketConnection(Socket socket)
        {
            ID = EncryptUtil.Md5.Encode(Guid.NewGuid().ToString());
            Socket = socket;
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public Task SendAsync(byte[] buffer)
        {
            return ThreadHelper.CreateTask(() => { Socket.Send(new ArraySegment<byte>(buffer)); });
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
