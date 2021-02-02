using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

using DwFramework.Core.Plugins;
using static DwFramework.Socket.SocketService;

namespace DwFramework.Socket
{
    public sealed class SocketConnection
    {
        public string ID { get; init; }
        public bool IsClose { get; private set; } = false;

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
                if (IsClose) return;
                var len = await _socket.ReceiveAsync(_buffer, SocketFlags.None);
                if (len > 0)
                {
                    var data = new byte[len];
                    Array.Copy(_buffer, data, len);
                    OnReceive?.Invoke(this, new OnReceiveEventargs() { Data = data });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) throw new Exception("空数据");
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) throw new Exception("空数据");
                await BeginReceive();
            }
            catch (SocketException ex)
            {
                switch (ex.SocketErrorCode)
                {
                    case SocketError.TryAgain:
                        await BeginReceive();
                        break;
                    default:
                        OnError?.Invoke(this, new OnErrorEventArgs() { Exception = ex });
                        Close();
                        break;
                }
            }
            catch (Exception ex)
            {
                OnError?.Invoke(this, new OnErrorEventArgs() { Exception = ex });
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
            if (IsClose) return;
            _socket.Close();
            IsClose = true;
            OnClose?.Invoke(this, new OnCloceEventargs() { });
        }
    }
}
