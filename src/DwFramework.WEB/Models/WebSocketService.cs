using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using DwFramework.Core;

namespace DwFramework.WEB
{
    public sealed class WebSocketService
    {
        private Config _config;
        private readonly Dictionary<string, WebSocketConnection> _connections = new Dictionary<string, WebSocketConnection>();
        private CancellationTokenSource _cancellationTokenSource;
        private event Action<IServiceCollection> _onConfigureServices;

        public event Action<WebSocketConnection, OnConnectEventArgs> OnConnect;
        public event Action<WebSocketConnection, OnCloceEventArgs> OnClose;
        public event Action<WebSocketConnection, OnSendEventArgs> OnSend;
        public event Action<WebSocketConnection, OnReceiveEventargs> OnReceive;
        public event Action<WebSocketConnection, OnErrorEventArgs> OnError;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="config"></param>
        public WebSocketService(ServiceHost host, Config config)
        {
            _config = config;
            host.ConfigureHostBuilder(hostBuilder =>
            {
                hostBuilder.ConfigureWebHost(webHostBuilder =>
                {
                    webHostBuilder.UseKestrel(options =>
                    {
                        if (config.Listens.ContainsKey("ws"))
                        {
                            var ipAndPort = config.Listens["ws"].Split(":");
                            var ip = string.IsNullOrEmpty(ipAndPort[0]) ? IPAddress.Any : IPAddress.Parse(ipAndPort[0]);
                            var port = int.Parse(ipAndPort[1]);
                            options.Listen(ip, port);
                        }
                        if (config.Listens.ContainsKey("wss"))
                        {
                            var addrAndCert = config.Listens["wss"].Split(";");
                            var ipAndPort = addrAndCert[0].Split(":");
                            var ip = string.IsNullOrEmpty(ipAndPort[0]) ? IPAddress.Any : IPAddress.Parse(ipAndPort[0]);
                            var port = int.Parse(ipAndPort[1]);
                            options.Listen(ip, port, listenOptions =>
                            {
                                var certAndPassword = addrAndCert[1].Split(",");
                                listenOptions.UseHttps(certAndPassword[0], certAndPassword[1]);
                            });
                        }
                    })
                    .Configure(app =>
                     {
                         app.UseWebSockets();
                         // 接受连接
                         app.Run(async context =>
                         {
                             if (!context.WebSockets.IsWebSocketRequest) return;
                             var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                             var connection = new WebSocketConnection(webSocket, _config.BufferSize, out var resetEvent)
                             {
                                 OnClose = OnClose,
                                 OnSend = OnSend,
                                 OnReceive = OnReceive,
                                 OnError = OnError
                             };
                             _connections[connection.ID] = connection;
                             OnConnect?.Invoke(connection, new OnConnectEventArgs() { Header = context.Request.Headers });
                             _ = connection.BeginReceiveAsync();
                             resetEvent.WaitOne();
                         });
                     });
                });
            });
        }

        /// <summary>
        /// 获取连接
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public WebSocketConnection GetSocketConnection(string id)
        {
            if (!_connections.ContainsKey(id)) return null;
            return _connections[id];
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
        /// <returns></returns>
        public Task CloseAsync(string id)
        {
            if (!_connections.ContainsKey(id)) return Task.CompletedTask;
            var connection = _connections[id];
            return connection.CloseAsync(WebSocketCloseStatus.NormalClosure).ContinueWith(task =>
            {
                if (!task.IsCompletedSuccessfully) OnError?.Invoke(connection, new OnErrorEventArgs() { Exception = task.Exception });
            });
        }

        /// <summary>
        /// 断开所有连接
        /// </summary>
        public void CloseAll()
        {
            foreach (var item in _connections.Keys) CloseAsync(item);
        }
    }
}
