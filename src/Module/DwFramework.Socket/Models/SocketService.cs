using System;
using System.Text;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Timers;
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

        /*
        /// <summary>
        /// 创建连接处理
        /// </summary>
        /// <param name="result"></param>
        private void OnConnectHandler(IAsyncResult result)
        {
            var socket = _server.EndAccept(result);
            var connection = new SocketConnection(socket, _config.BufferSize);
            _server.BeginAccept(OnConnectHandler, null);
            _connections[connection.ID] = connection;
            OnConnect?.Invoke(connection, new OnConnectEventargs() { });
            var data = new byte[_config.BufferSize];
            socket.BeginReceive(data, 0, _config.BufferSize, SocketFlags.None, OnReceiveHandler, (connection, data));
        }

        /// <summary>
        /// 接收消息处理
        /// </summary>
        /// <param name="result"></param>
        private void OnReceiveHandler(IAsyncResult result)
        {
            var (connection, data) = ((SocketConnection, byte[]))result.AsyncState;
            SocketError code = SocketError.Success;
            try
            {
                var len = connection.Socket.EndReceive(result, out code);
                var o = connection.Socket.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Error);
                if (len > 0)
                {
                    Array.Resize(ref data, len);
                    OnReceive?.Invoke(connection, new OnReceiveEventargs() { Data = data });
                }
                if (!connection.IsAvailable()) throw new Exception("连接不可用");
                var newData = new byte[_config.BufferSize];
                connection.Socket.BeginReceive(newData, 0, _config.BufferSize, SocketFlags.None, OnReceiveHandler, (connection, newData));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                switch (code)
                {
                    default:
                        _ = CloseAsync(connection);
                        return;
                }
            }
        }

        /// <summary>
        /// 检查连接状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void CheckConnection(object sender, EventArgs args)
        {
            if (_isChecking) return;
            var currentTime = DateTime.UtcNow;
            _isChecking = true;
            _connections.Values.ForEach(item =>
            {
                //Console.WriteLine(item.IsAvailable());
                if (item.IsAvailable()) return;
                _ = CloseAsync(item);
            });
            _isChecking = false;
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task SendAsync(SocketConnection connection, byte[] data)
        {
            if (!connection.IsAvailable())
            {
                _ = CloseAsync(connection);
                throw new Exception("该客户端状态异常");
            }
            await connection.SendAsync(data);
            OnSend?.Invoke(connection, new OnSendEventargs() { Data = data });
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task SendAsync(string id, byte[] data)
        {
            if (!_connections.ContainsKey(id)) throw new Exception("该客户端不存在");
            await SendAsync(_connections[id], data);
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="msg"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public async Task SendAsync(string id, string msg, Encoding encoding = null)
        {
            encoding ??= Encoding.UTF8;
            await SendAsync(id, encoding.GetBytes(msg));
        }

        /// <summary>
        /// 广播消息
        /// </summary>
        /// <param name="data"></param>
        /// <param name="onException"></param>
        /// <returns></returns>
        public Task BroadCastAsync(byte[] data, Action<string, Exception> onException = null)
        {
            return TaskManager.CreateTask(() => _connections.Values.ForEach(async item => await SendAsync(item.ID, data),
                onException == null ? null : (connection, ex) => onException?.Invoke(connection.ID, ex)));
        }

        /// <summary>
        /// 广播消息
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="encoding"></param>
        /// <param name="onException"></param>
        /// <returns></returns>
        public Task BroadCastAsync(string msg, Encoding encoding = null, Action<string, Exception> onException = null)
        {
            encoding ??= Encoding.UTF8;
            return BroadCastAsync(encoding.GetBytes(msg), onException);
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public async Task CloseAsync(SocketConnection connection)
        {
            if (_connections.ContainsKey(connection.ID)) _connections.Remove(connection.ID);
            await connection.CloseAsync();
            OnClose?.Invoke(connection, new OnCloceEventargs() { });
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task CloseAsync(string id)
        {
            if (!_connections.ContainsKey(id)) return;
            await CloseAsync(_connections[id]);
        }

        /// <summary>
        /// 断开所有连接
        /// </summary>
        /// <returns></returns>
        public void CloseAll()
        {
            _connections.Values.ForEach(async item => await item.CloseAsync());
        }
        */
    }
}
