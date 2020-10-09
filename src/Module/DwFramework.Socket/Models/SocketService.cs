using System;
using System.Text;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

using DwFramework.Core;
using DwFramework.Core.Plugins;

namespace DwFramework.Socket
{
    public sealed class SocketService
    {
        public class Config
        {
            public string Listen { get; set; }
            public int BackLog { get; set; } = 100;
            public int BufferSize { get; set; } = 1024 * 4;
        }

        public class OnConnectEventargs : EventArgs
        {

        }

        public class OnSendEventargs : EventArgs
        {
            public byte[] Data { get; private set; }

            public OnSendEventargs(byte[] data)
            {
                Data = data;
            }
        }

        public class OnReceiveEventargs : EventArgs
        {
            public byte[] Data { get; private set; }

            public OnReceiveEventargs(byte[] data)
            {
                Data = data;
            }
        }

        public class OnCloceEventargs : EventArgs
        {

        }

        private readonly Config _config;
        private readonly Dictionary<string, SocketConnection> _connections;
        private System.Net.Sockets.Socket _server;

        public event Action<SocketConnection, OnConnectEventargs> OnConnect;
        public event Action<SocketConnection, OnSendEventargs> OnSend;
        public event Action<SocketConnection, OnReceiveEventargs> OnReceive;
        public event Action<SocketConnection, OnCloceEventargs> OnClose;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="configKey"></param>
        public SocketService(Core.Environment environment, string configKey = null)
        {
            var configuration = environment.GetConfiguration(configKey ?? "Socket");
            _config = configuration.GetConfig<Config>(configKey);
            if (_config == null) throw new Exception("未读取到Socket配置");
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
                Console.WriteLine($"Socket服务已开启 => 监听地址:{_config.Listen}");
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
            OnReceive?.Invoke(connection, new OnReceiveEventargs(data));
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
        /// <param name="buffer"></param>
        /// <returns></returns>
        public Task SendAsync(string id, byte[] buffer)
        {
            RequireClient(id);
            var connection = _connections[id];
            return connection.SendAsync(buffer)
                .ContinueWith(a => OnSend?.Invoke(connection, new OnSendEventargs(buffer)));
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public Task SendAsync(string id, string msg)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(msg);
            return SendAsync(id, buffer);
        }

        /// <summary>
        /// 广播消息
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public Task BroadCastAsync(string msg)
        {
            return TaskManager.CreateTask(() =>
            {
                byte[] buffer = Encoding.UTF8.GetBytes(msg);
                foreach (var item in _connections.Values)
                {
                    SendAsync(item.ID, buffer);
                }
            });
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task SocketCloseAsync(string id)
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
                foreach (var item in _connections.Values)
                {
                    item.CloseAsync();
                }
            });
        }
    }
}
