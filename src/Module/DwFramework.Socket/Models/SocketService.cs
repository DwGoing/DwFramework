using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

using DwFramework.Core;
using DwFramework.Core.Extensions;
using DwFramework.Core.Plugins;

namespace DwFramework.Socket
{
    public sealed class SocketService
    {
        public class Config
        {
            public string Listen { get; init; }
            public int BackLog { get; init; } = 100;
            public int BufferSize { get; init; } = 1024 * 4;
        }

        public class OnConnectEventargs : EventArgs
        {

        }

        public class OnCloceEventargs : EventArgs
        {

        }

        public class OnSendEventargs : EventArgs
        {
            public byte[] Data { get; init; }
        }

        public class OnReceiveEventargs : EventArgs
        {
            public byte[] Data { get; init; }
        }

        public class OnErrorEventArgs : EventArgs
        {
            public Exception Exception { get; init; }
        }

        private readonly Config _config;
        private readonly ILogger<SocketService> _logger;
        private readonly Dictionary<string, SocketConnection> _connections;
        private System.Net.Sockets.Socket _server;

        public event Action<SocketConnection, OnConnectEventargs> OnConnect;
        public event Action<SocketConnection, OnCloceEventargs> OnClose;
        public event Action<SocketConnection, OnSendEventargs> OnSend;
        public event Action<SocketConnection, OnReceiveEventargs> OnReceive;
        public event Action<SocketConnection, OnErrorEventArgs> OnError;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="path"></param>
        /// <param name="key"></param>
        public SocketService(string path = null, string key = null)
        {
            _config = ServiceHost.Environment.GetConfiguration<Config>(path, key);
            if (_config == null) throw new Exception("未读取到Socket配置");
            _logger = ServiceHost.Provider.GetLogger<SocketService>();
            _connections = new Dictionary<string, SocketConnection>();
        }

        /// <summary>
        /// 开启服务
        /// </summary>
        /// <returns></returns>
        public async Task OpenServiceAsync()
        {
            if (_config.Listen == null) throw new Exception("缺少Listen配置");
            _server = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var ipAndPort = _config.Listen.Split(":");
            _server.Bind(new IPEndPoint(string.IsNullOrEmpty(ipAndPort[0]) ? IPAddress.Any : IPAddress.Parse(ipAndPort[0]), int.Parse(ipAndPort[1])));
            _server.Listen(_config.BackLog);
            OnClose += OnCloseHandler;
            _ = BeginAccept();
            await _logger?.LogInformationAsync($"Socket服务正在监听:{_config.Listen}");
        }

        /// <summary>
        /// 开始接受连接
        /// </summary>
        /// <returns></returns>
        private async Task BeginAccept()
        {
            try
            {
                var socket = await _server.AcceptAsync();
                var connection = new SocketConnection(socket, _config.BufferSize)
                {
                    OnClose = OnClose,
                    OnSend = OnSend,
                    OnReceive = OnReceive,
                    OnError = OnError
                };
                _connections[connection.ID] = connection;
                OnConnect?.Invoke(connection, new OnConnectEventargs() { });
                await BeginAccept();
            }
            catch (Exception ex)
            {
                await _logger?.LogErrorAsync($"Socket服务异常:{ex.Message}");
                OnError?.Invoke(null, new OnErrorEventArgs() { Exception = ex });
            }
        }

        /// <summary>
        /// 连接关闭时处理
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="args"></param>
        private void OnCloseHandler(SocketConnection connection, OnCloceEventargs args)
        {
            if (_connections.ContainsKey(connection.ID)) _connections.Remove(connection.ID);
        }

        /// <summary>
        /// 获取连接
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SocketConnection GetSocketConnection(string id)
        {
            if (!_connections.ContainsKey(id)) return null;
            return _connections[id];
        }

        /// <summary>
        /// 广播消息
        /// </summary>
        /// <param name="data"></param>
        public void BroadCast(byte[] data)
        {
            _connections.Values.ForEach(async item => await item.SendAsync(data), (connection, ex) =>
            {
                OnError?.Invoke(connection, new OnErrorEventArgs() { Exception = ex });
            });
        }

        /// <summary>
        /// 断开所有连接
        /// </summary>
        public void CloseAll()
        {
            _connections.Values.ForEach(item => item.Close(), (connection, ex) =>
            {
                OnError?.Invoke(connection, new OnErrorEventArgs() { Exception = ex });
            });
        }
    }
}
