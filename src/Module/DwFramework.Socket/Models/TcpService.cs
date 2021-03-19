using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;

using DwFramework.Core;
using DwFramework.Core.Plugins;
using DwFramework.Core.Extensions;

namespace DwFramework.Socket
{
    public sealed class TcpService : ConfigableService
    {
        public sealed class Config
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

        private readonly ILogger<TcpService> _logger;
        private Config _config;
        private readonly Dictionary<string, TcpConnection> _connections = new Dictionary<string, TcpConnection>();
        private System.Net.Sockets.Socket _server;

        public event Action<TcpConnection, OnConnectEventargs> OnConnect;
        public event Action<TcpConnection, OnCloceEventargs> OnClose;
        public event Action<TcpConnection, OnSendEventargs> OnSend;
        public event Action<TcpConnection, OnReceiveEventargs> OnReceive;
        public event Action<TcpConnection, OnErrorEventArgs> OnError;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger"></param>
        public TcpService(ILogger<TcpService> logger = null)
        {
            _logger = logger;
        }

        /// <summary>
        /// 读取配置
        /// </summary>
        /// <param name="config"></param>
        public void ReadConfig(Config config)
        {
            try
            {
                _config = config;
                if (_config == null) throw new Exception("未读取到TCP配置");
            }
            catch (Exception ex)
            {
                _ = _logger?.LogErrorAsync(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// 读取配置
        /// </summary>
        /// <param name="path"></param>
        /// <param name="key"></param>
        public void ReadConfig(string path = null, string key = null)
        {
            ReadConfig(ReadConfig<Config>(path, key));
        }

        /// <summary>
        /// 运行服务
        /// </summary>
        /// <returns></returns>
        public async Task RunAsync()
        {
            try
            {
                if (_config.Listen == null) throw new Exception("缺少Listen配置");
                _server = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                var ipAndPort = _config.Listen.Split(":");
                _server.Bind(new IPEndPoint(string.IsNullOrEmpty(ipAndPort[0]) ? IPAddress.Any : IPAddress.Parse(ipAndPort[0]), int.Parse(ipAndPort[1])));
                _server.Listen(_config.BackLog);
                OnClose += OnCloseHandler;
                if (_logger != null) await _logger?.LogInformationAsync($"Tcp服务正在监听:{_config.Listen}");
                await BeginAcceptAsync();
            }
            catch (Exception ex)
            {
                if (_logger != null) await _logger?.LogErrorAsync(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        public void Stop()
        {
            _server.Dispose();
            _connections.Clear();
        }

        /// <summary>
        /// 开始接受连接
        /// </summary>
        /// <returns></returns>
        private async Task BeginAcceptAsync()
        {
            TcpConnection connection = null;
            try
            {
                var socket = await _server.AcceptAsync();
                connection = new TcpConnection(socket, _config.BufferSize)
                {
                    OnClose = OnClose,
                    OnSend = OnSend,
                    OnReceive = OnReceive,
                    OnError = OnError
                };
                _connections[connection.ID] = connection;
                OnConnect?.Invoke(connection, new OnConnectEventargs() { });
                _ = connection.BeginReceiveAsync();
                await BeginAcceptAsync();
            }
            catch (Exception ex)
            {
                if (_logger != null) await _logger?.LogErrorAsync($"Tcp服务异常:{ex.Message}");
                OnError?.Invoke(connection, new OnErrorEventArgs() { Exception = ex });
            }
        }

        /// <summary>
        /// 连接关闭时处理
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="args"></param>
        private void OnCloseHandler(TcpConnection connection, OnCloceEventargs args)
        {
            if (_connections.ContainsKey(connection.ID)) _connections.Remove(connection.ID);
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
