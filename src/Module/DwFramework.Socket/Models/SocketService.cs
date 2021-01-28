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
        public async Task OpenServiceAsync()
        {
            _server = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            if (_config.Listen == null) throw new Exception("缺少Listen配置");
            var ipAndPort = _config.Listen.Split(":");
            _server.Bind(new IPEndPoint(string.IsNullOrEmpty(ipAndPort[0]) ? IPAddress.Any : IPAddress.Parse(ipAndPort[0]), int.Parse(ipAndPort[1])));
            _server.Listen(_config.BackLog);
            var acceptArgs = new SocketAsyncEventArgs();
            acceptArgs.Completed += OnConnectHandler;
            _server.AcceptAsync(acceptArgs);
            await _logger?.LogInformationAsync($"Socket服务正在监听:{_config.Listen}");
        }

        /// <summary>
        /// 创建连接处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnConnectHandler(object sender, SocketAsyncEventArgs args)
        {
            var connection = new SocketConnection(args.AcceptSocket);
            args.AcceptSocket = null;
            _server.AcceptAsync(args);
            _connections[connection.ID] = connection;
            OnConnect?.Invoke(connection, new OnConnectEventargs() { });
            //var receiveArgs = new SocketAsyncEventArgs();
            //receiveArgs.Completed += (_, e) => Console.WriteLine("==============");
            //receiveArgs.SetBuffer(new byte[_config.BufferSize], 0, _config.BufferSize);
            //if (!connection.Socket.ReceiveAsync(receiveArgs)) OnReceiveHandler(connection, receiveArgs);
        }

        /// <summary>
        /// 接收消息处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnReceiveHandler(SocketConnection connection, SocketAsyncEventArgs args)
        {
            var isClose = false;
            switch (args.SocketError)
            {
                case SocketError.Success:
                    if (args.BytesTransferred > 0)
                    {
                        var data = new byte[args.BytesTransferred];
                        Array.Copy(args.Buffer, data, args.BytesTransferred);
                        OnReceive?.Invoke(connection, new OnReceiveEventargs() { Data = data });
                    }
                    break;
                default:
                    OnCloseHandler(connection);
                    isClose = true;
                    break;
            };
            if (!isClose)
            {
                var receiveArgs = new SocketAsyncEventArgs();
                receiveArgs.SetBuffer(new byte[_config.BufferSize], 0, _config.BufferSize);
                if (!connection.Socket.ReceiveAsync(receiveArgs)) OnReceiveHandler(connection, receiveArgs);
            }
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
            //if (!connection.CheckConnection())
            //    throw new Exception("该客户端状态错误");
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task SendAsync(string id, byte[] data)
        {
            RequireClient(id);
            var connection = _connections[id];
            await connection.SendAsync(data);
            OnSend?.Invoke(connection, new OnSendEventargs() { Data = data });
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
        /// <returns></returns>
        public void BroadCast(byte[] data)
        {
            _connections.Values.ForEach(async item => await SendAsync(item.ID, data));
        }

        /// <summary>
        /// 广播消息
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public void BroadCast(string msg, Encoding encoding = null)
        {
            encoding ??= Encoding.UTF8;
            BroadCast(encoding.GetBytes(msg));
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task CloseAsync(string id)
        {
            RequireClient(id);
            var connection = _connections[id];
            await connection.CloseAsync();
        }

        /// <summary>
        /// 断开所有连接
        /// </summary>
        /// <returns></returns>
        public void CloseAll()
        {
            _connections.Values.ForEach(async item => await item.CloseAsync());
        }
    }
}
