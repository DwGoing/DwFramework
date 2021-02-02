using System;
using System.Net.Sockets;
using System.Threading.Tasks;

using DwFramework.Core.Plugins;
using static DwFramework.Socket.SocketService;

namespace DwFramework.Socket
{
    public sealed class SocketConnection
    {
        public string ID { get; init; }

        private readonly System.Net.Sockets.Socket _socket;
        private readonly byte[] _buffer;

        public Action<SocketConnection, OnCloceEventargs> OnClose { get; init; }
        public Action<SocketConnection, OnSendEventargs> OnSend { get; init; }
        public Action<SocketConnection, OnReceiveEventargs> OnReceive { get; init; }
        public Action<SocketConnection, OnErrorEventArgs> OnError { get; init; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="socket"></param>
        public SocketConnection(System.Net.Sockets.Socket socket, int bufferSize)
        {
            ID = MD5.Encode(Guid.NewGuid().ToString());
            _socket = socket;
            _socket.EnableKeepAlive(3000, 500);
            _buffer = new byte[bufferSize];
            _ = BeginReceive();
        }

        /// <summary>
        /// 开始接收数据
        /// </summary>
        /// <returns></returns>
        private async Task BeginReceive()
        {
            try
            {
                var len = await _socket.ReceiveAsync(_buffer, SocketFlags.None);
                var a = _socket.LingerState;
                if (len > 0)
                {
                    var data = new byte[len];
                    Array.Copy(_buffer, data, len);
                    OnReceive?.Invoke(this, new OnReceiveEventargs() { Data = data });
                }
                await BeginReceive();
            }
            catch
            {
                Close();
            }
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public async Task<int> SendAsync(byte[] buffer)
        {
            try
            {
                return await _socket.SendAsync(buffer, SocketFlags.None);
            }
            catch
            {
                Close();
                return 0;
            }
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        public void Close()
        {
            try
            {
                _socket.Shutdown(SocketShutdown.Both);
            }
            finally
            {
                _socket.Close();
            }
        }
    }
}
