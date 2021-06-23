using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using DwFramework.Core;

namespace DwFramework.WEB
{
    public sealed class SocketService
    {
        private Config _config;
        private readonly Dictionary<string, TcpConnection> _connections = new Dictionary<string, TcpConnection>();
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
                    if (!config.Listens.ContainsKey("tcp")) throw new Exception("缺少Listens配置");
                    var ipAndPort = _config.Listens["tcp"].Split(":");
                    _server.Bind(new IPEndPoint(string.IsNullOrEmpty(ipAndPort[0]) ? IPAddress.Any : IPAddress.Parse(ipAndPort[0]), int.Parse(ipAndPort[1])));
                    _server.Listen(_config.BackLog);
                    break;
                case ProtocolType.Udp:

                    break;
                default:
                    throw new Exception("未定义协议类型");
            }

        }
    }
}