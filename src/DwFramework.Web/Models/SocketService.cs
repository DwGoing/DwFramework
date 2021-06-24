using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace DwFramework.Web
{
    public sealed class SocketService
    {
        private Config _config;
        private readonly Dictionary<string, TcpConnection> _connections = new();
        private Socket _server;

        public event Action<TcpConnection, OnConnectEventArgs> OnConnect;
        public event Action<TcpConnection, OnCloceEventArgs> OnClose;
        public event Action<TcpConnection, OnSendEventArgs> OnSend;
        public event Action<TcpConnection, OnReceiveEventArgs> OnReceive;
        public event Action<TcpConnection, OnErrorEventArgs> OnError;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="config"></param>
        public SocketService(Config config)
        {
            _config = config;
            switch (config.ProtocolType)
            {
                case ProtocolType.Tcp:
                    _server = new Socket(config.AddressFamily, config.SocketType, config.ProtocolType);
                    StartTcpServiceAsync();
                    break;
                case ProtocolType.Udp:

                    break;
                default:
                    throw new Exception("未定义协议类型");
            }
        }

        /// <summary>
        /// 开始TCP服务
        /// </summary>
        private void StartTcpServiceAsync()
        {
            async Task AcceptAsync()
            {
                var socket = await _server.AcceptAsync();
                _ = AcceptAsync();
                if (socket == null) return;
                var connection = new TcpConnection(socket, _config.BufferSize)
                {
                    OnClose = OnClose,
                    OnSend = OnSend,
                    OnReceive = OnReceive,
                    OnError = OnError
                };
                _connections[connection.ID] = connection;
                OnConnect?.Invoke(connection, new OnConnectEventArgs() { });
                _ = connection.BeginReceiveAsync();
            }

            if (!_config.Listens.ContainsKey("tcp")) throw new Exception("缺少Listens配置");
            var ipAndPort = _config.Listens["tcp"].Split(":");
            var ip = string.IsNullOrEmpty(ipAndPort[0]) ? IPAddress.Any : IPAddress.Parse(ipAndPort[0]);
            var port = int.Parse(ipAndPort[1]);
            _server.Bind(new IPEndPoint(ip, port));
            _server.Listen(_config.BackLog);
            _ = AcceptAsync();
        }
    }
}