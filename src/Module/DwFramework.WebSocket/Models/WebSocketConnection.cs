using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using System.Text;

using DwFramework.Plugins.Core;

namespace DwFramework.WebSocket
{
    public class WebSocketConnection
    {
        public readonly string ID;
        public readonly System.Net.WebSockets.WebSocket WebSocket;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="webSocket"></param>
        public WebSocketConnection(System.Net.WebSockets.WebSocket webSocket)
        {
            ID = MD5.Encode(Guid.NewGuid().ToString());
            WebSocket = webSocket;
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public Task SendAsync(byte[] buffer)
        {
            return WebSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
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
            return WebSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None).ContinueWith(a => Dispose());
        }

        /// <summary>
        /// 释放连接
        /// </summary>
        public void Dispose()
        {
            WebSocket.Abort();
            WebSocket.Dispose();
        }
    }
}
