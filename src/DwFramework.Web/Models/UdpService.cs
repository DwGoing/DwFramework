using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Configuration;
using DwFramework.Core;

namespace DwFramework.Web.Socket
{
    public sealed class UdpService
    {
        private readonly Config.Socket _config;
        private readonly Dictionary<string, TcpConnection> _connections = new();
        private readonly System.Net.Sockets.Socket _server;
        private byte[] _buffer;

        public event Action<EndPoint, OnSendEventArgs> OnSend;
        public event Action<EndPoint, OnReceiveEventArgs> OnReceive;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="configuration"></param>
        public UdpService(IConfiguration configuration)
        {
            _config = configuration.ParseConfiguration<Config.Socket>();
            if (_config.Listen == null) throw new NotFoundException("缺少Socket配置");
            _buffer = new byte[_config.BufferSize > 0 ? _config.BufferSize : 4096];
            _server = new System.Net.Sockets.Socket(_config.AddressFamily, _config.SocketType, ProtocolType.Udp);
            if (_config.Listen == null) throw new NotFoundException("缺少Listen配置");
            _server.Bind(new IPEndPoint(string.IsNullOrEmpty(_config.Listen.Ip) ? IPAddress.Any : IPAddress.Parse(_config.Listen.Ip), _config.Listen.Port));
            _ = AcceptAsync();
        }

        /// <summary>
        /// 开始UDP服务
        /// </summary>
        private async Task AcceptAsync()
        {
            var remote = new IPEndPoint(IPAddress.Any, 0);
            var result = await _server.ReceiveFromAsync(_buffer, SocketFlags.None, remote);
            _ = AcceptAsync();
            var len = result.ReceivedBytes;
            if (len > 0)
            {
                var data = new byte[len];
                Array.Copy(_buffer, data, len);
                OnReceive?.Invoke(result.RemoteEndPoint, new OnReceiveEventArgs() { Data = data });
            }
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="data"></param>
        /// <param name="remote"></param>
        /// <returns></returns>
        public int SendTo(byte[] data, EndPoint remote)
        {
            var len = _server.SendTo(data, remote);
            OnSend?.Invoke(remote, new OnSendEventArgs() { Data = data });
            return len;
        }
    }
}