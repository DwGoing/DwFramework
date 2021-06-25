using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace DwFramework.Web.Socket
{
    public sealed class UdpService
    {
        private Config _config;
        private readonly Dictionary<string, TcpConnection> _connections = new();
        private readonly System.Net.Sockets.Socket _server;
        private byte[] _buffer;

        public event Action<EndPoint, OnSendEventArgs> OnSend;
        public event Action<EndPoint, OnReceiveEventArgs> OnReceive;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="config"></param>
        public UdpService(Config config)
        {
            _config = config;
            _buffer = new byte[_config.BufferSize > 0 ? _config.BufferSize : 4096];
            _server = new System.Net.Sockets.Socket(config.AddressFamily, config.SocketType, ProtocolType.Udp);

            if (!_config.Listens.ContainsKey("udp")) throw new Exception("缺少Listens配置");
            var ipAndPort = _config.Listens["udp"].Split(":");
            var ip = string.IsNullOrEmpty(ipAndPort[0]) ? IPAddress.Any : IPAddress.Parse(ipAndPort[0]);
            var port = int.Parse(ipAndPort[1]);
            _server.Bind(new IPEndPoint(ip, port));
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