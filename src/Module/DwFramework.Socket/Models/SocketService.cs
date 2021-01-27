using System;
using System.Text;
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

        public class OnSendEventargs : EventArgs
        {
            public byte[] Data { get; init; }
        }

        public class OnReceiveEventargs : EventArgs
        {
            public byte[] Data { get; init; }
        }

        public class OnCloceEventargs : EventArgs
        {

        }

        private readonly Config _config;
        private readonly ILogger<SocketService> _logger;
        private readonly Dictionary<string, SocketConnection> _connections;
        private System.Net.Sockets.Socket _server;

        public event Action<SocketConnection, OnConnectEventargs> OnConnect;
        public event Action<SocketConnection, OnSendEventargs> OnSend;
        public event Action<SocketConnection, OnReceiveEventargs> OnReceive;
        public event Action<SocketConnection, OnCloceEventargs> OnClose;

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
        public Task OpenServiceAsync()
        {
            return TaskManager.CreateTask(() =>
            {
                _server = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                if (_config.Listen == null) throw new Exception("缺少Listen配置");
                string[] ipAndPort = _config.Listen.Split(":");
                _server.Bind(new IPEndPoint(string.IsNullOrEmpty(ipAndPort[0]) ? IPAddress.Any : IPAddress.Parse(ipAndPort[0]), int.Parse(ipAndPort[1])));
                _server.Listen(_config.BackLog);
                _server.BeginAccept(OnConnectHandler, _server);
                _logger?.LogInformationAsync($"Socket服务正在监听:{_config.Listen}");
            });
        }

        /// <summary>
        /// 创建连接处理
        /// </summary>
        /// <param name="ar"></param>
        private void OnConnectHandler(IAsyncResult ar)
        {
            var connection = new SocketConnection(_server.EndAccept(ar), _config.BufferSize);
            _server.BeginAccept(OnConnectHandler, _server);
            _connections[connection.ID] = connection;
            OnConnect?.Invoke(connection, new OnConnectEventargs() { });
            connection.Socket.BeginReceive(connection.Buffer, 0, connection.Buffer.Length, SocketFlags.None, OnReceiveHandler, connection);
        }

        /// <summary>
        /// 接收消息处理
        /// </summary>
        /// <param name="ar"></param>
        private void OnReceiveHandler(IAsyncResult ar)
        {
            var connection = ar.AsyncState as SocketConnection;
            if (!connection.CheckConnection())
            {
                OnCloseHandler(connection);
                return;
            }
            var len = connection.Socket.EndReceive(ar);
            var message = Encoding.UTF8.GetString(connection.Buffer, 0, len);
            var data = new byte[len];
            Array.Copy(connection.Buffer, data, len);
            OnReceive?.Invoke(connection, new OnReceiveEventargs() { Data = data });
            connection.Socket.BeginReceive(connection.Buffer, 0, connection.Buffer.Length, SocketFlags.None, OnReceiveHandler, connection);
        }

        /// <summary>
        /// 关闭连接处理
        /// </summary>
        /// <param name="connection"></param>
        private void OnCloseHandler(SocketConnection connection)
        {
            OnClose?.Invoke(connection, new OnCloceEventargs() { });
            connection.Dispose();
            _connections.Remove(connection.ID);
        }

        /// <summary>
        /// 检查客户端
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private void RequireClient(string id)
        {
            if (!_connections.ContainsKey(id))
                throw new Exception("该客户端不存在");
            var connection = _connections[id];
            if (!connection.CheckConnection())
                throw new Exception("该客户端状态错误");
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public Task SendAsync(string id, byte[] data)
        {
            RequireClient(id);
            var connection = _connections[id];
            return connection.SendAsync(data).ContinueWith(_ => OnSend?.Invoke(connection, new OnSendEventargs() { Data = data }));
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="msg"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public Task SendAsync(string id, string msg, Encoding encoding = null)
        {
            encoding ??= Encoding.UTF8;
            return SendAsync(id, encoding.GetBytes(msg));
        }

        /// <summary>
        /// 广播消息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public Task BroadCastAsync(byte[] data)
        {
            return TaskManager.CreateTask(() =>
            {
                _connections.Values.ForEach(item => SendAsync(item.ID, data), (_, _) => { });
            });
        }

        /// <summary>
        /// 广播消息
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public Task BroadCastAsync(string msg, Encoding encoding = null)
        {
            encoding ??= Encoding.UTF8;
            return BroadCastAsync(encoding.GetBytes(msg));
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task CloseAsync(string id)
        {
            RequireClient(id);
            var connection = _connections[id];
            return connection.CloseAsync();
        }

        /// <summary>
        /// 断开所有连接
        /// </summary>
        /// <returns></returns>
        public Task CloseAllAsync()
        {
            return TaskManager.CreateTask(() =>
            {
                _connections.Values.ForEach(item => item.CloseAsync(), (_, _) => { });
            });
        }
    }
}
