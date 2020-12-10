using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

using DwFramework.Core.Plugins;

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
        public async Task SendAsync(byte[] buffer) => await WebSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <returns></returns>
        public async Task CloseAsync() => await WebSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None).ContinueWith(task => Dispose());

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
