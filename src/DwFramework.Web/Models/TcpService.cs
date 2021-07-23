using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Configuration;
using DwFramework.Core;

namespace DwFramework.Web.Socket
{
    public sealed class TcpService
    {
        private readonly Config.Socket _config;
        private readonly Dictionary<string, TcpConnection> _connections = new();
        private System.Net.Sockets.Socket _server;

        public event Action<TcpConnection, OnConnectEventArgs> OnConnect;
        public event Action<TcpConnection, OnCloceEventArgs> OnClose;
        public event Action<TcpConnection, OnSendEventArgs> OnSend;
        public event Action<TcpConnection, OnReceiveEventArgs> OnReceive;
        public event Action<TcpConnection, OnErrorEventArgs> OnError;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="configuration"></param>
        public TcpService(IConfiguration configuration)
        {
            _config = configuration.ParseConfiguration<Config.Socket>();
            if (_config.Listen == null) throw new NotFoundException("缺少Socket配置");
            _server = new System.Net.Sockets.Socket(_config.AddressFamily, _config.SocketType, ProtocolType.Tcp);
            if (_config.Listen == null) throw new NotFoundException("缺少Listen配置");
            _server.Bind(new IPEndPoint(string.IsNullOrEmpty(_config.Listen.Ip) ? IPAddress.Any : IPAddress.Parse(_config.Listen.Ip), _config.Listen.Port));
            _server.Listen(_config.BackLog);
            _ = AcceptAsync();
        }

        /// <summary>
        /// 开始TCP服务
        /// </summary>
        private async Task AcceptAsync()
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

        /// <summary>
        /// 获取连接
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TcpConnection GetSocketConnection(string id)
        {
            if (!_connections.ContainsKey(id)) return null;
            return _connections[id];
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public Task SendAsync(string id, byte[] data)
        {
            if (!_connections.ContainsKey(id)) return Task.CompletedTask;
            return _connections[id].SendAsync(data);
        }

        /// <summary>
        /// 广播消息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public void BroadCast(byte[] data)
        {
            foreach (var item in _connections.Values)
            {
                item.SendAsync(data).ContinueWith(task =>
                {
                    if (!task.IsCompletedSuccessfully) OnError?.Invoke(item, new OnErrorEventArgs() { Exception = task.Exception });
                });
            }
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <param name="id"></param>
        public void Close(string id)
        {
            if (!_connections.ContainsKey(id)) return;
            var connection = _connections[id];
            connection.Close();
        }
    }
}