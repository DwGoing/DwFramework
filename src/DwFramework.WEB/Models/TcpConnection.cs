using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using DwFramework.Core.Encrypt;

namespace DwFramework.WEB
{
    public sealed class TcpConnection
    {
        public string ID { get; init; }
        public bool IsClose { get; private set; } = false;

        private readonly Socket _socket;
        private readonly byte[] _buffer;

        public Action<TcpConnection, OnCloceEventArgs> OnClose { get; init; }
        public Action<TcpConnection, OnSendEventArgs> OnSend { get; init; }
        public Action<TcpConnection, OnReceiveEventArgs> OnReceive { get; init; }
        public Action<TcpConnection, OnErrorEventArgs> OnError { get; init; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="bufferSize"></param>
        public TcpConnection(Socket socket, int bufferSize)
        {
            ID = MD5.Encode(Guid.NewGuid().ToString());
            _socket = socket;
            _socket.EnableKeepAlive(3000, 500);
            _buffer = new byte[bufferSize];
        }

        /// <summary>
        /// 开始接收数据
        /// </summary>
        /// <returns></returns>
        public async Task BeginReceiveAsync()
        {
            try
            {
                if (IsClose) return;
                if (!_socket.Connected) throw new SocketException((int)SocketError.NotConnected);
                var len = await _socket.ReceiveAsync(_buffer, SocketFlags.None);
                if (len > 0)
                {
                    var data = new byte[len];
                    Array.Copy(_buffer, data, len);
                    OnReceive?.Invoke(this, new OnReceiveEventArgs() { Data = data });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) throw new SocketException((int)SocketError.Disconnecting);
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) throw new SocketException((int)SocketError.Disconnecting);
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) throw new SocketException((int)SocketError.Disconnecting);
                await BeginReceiveAsync();
            }
            catch (SocketException ex)
            {
                switch (ex.SocketErrorCode)
                {
                    // TODO
                    default:
                        OnError?.Invoke(this, new OnErrorEventArgs() { Exception = ex });
                        Close();
                        break;
                }
            }
            catch (Exception ex)
            {
                OnError?.Invoke(this, new OnErrorEventArgs() { Exception = ex });
                await BeginReceiveAsync();
            }
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<int> SendAsync(byte[] data)
        {
            try
            {
                var len = await _socket.SendAsync(data, SocketFlags.None);
                OnSend?.Invoke(this, new OnSendEventArgs() { Data = data });
                return len;
            }
            catch (Exception ex)
            {
                OnError?.Invoke(this, new OnErrorEventArgs() { Exception = ex });
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
            OnClose?.Invoke(this, new OnCloceEventArgs() { });
        }
    }
}
