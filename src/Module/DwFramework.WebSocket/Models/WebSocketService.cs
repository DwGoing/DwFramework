﻿using System;
using System.Threading;
using System.Text;
using System.Collections.Generic;
using System.Net;
using System.Net.WebSockets;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using DwFramework.Core;
using DwFramework.Core.Plugins;
using DwFramework.Core.Extensions;

namespace DwFramework.WebSocket
{
    public sealed class WebSocketService
    {
        public class Config
        {
            public string ContentRoot { get; set; }
            public Dictionary<string, string> Listen { get; set; }
            public int BufferSize { get; set; } = 1024 * 4;
        }

        public class OnConnectEventargs : EventArgs
        {
            public IHeaderDictionary Header { get; private set; }

            public OnConnectEventargs(IHeaderDictionary header)
            {
                Header = header;
            }
        }

        public class OnSendEventargs : EventArgs
        {
            public string Message { get; private set; }

            public OnSendEventargs(string msg)
            {
                Message = msg;
            }
        }

        public class OnReceiveEventargs : EventArgs
        {
            public string Message { get; private set; }

            public OnReceiveEventargs(string msg)
            {
                Message = msg;
            }
        }

        public class OnCloceEventargs : EventArgs
        {

        }

        public class OnErrorEventargs : EventArgs
        {
            public Exception Exception { get; private set; }

            public OnErrorEventargs(Exception exception)
            {
                Exception = exception;
            }
        }

        private readonly Config _config;
        private readonly ILogger<WebSocketService> _logger;
        private readonly Dictionary<string, WebSocketConnection> _connections;

        public event Action<WebSocketConnection, OnConnectEventargs> OnConnect;
        public event Action<WebSocketConnection, OnSendEventargs> OnSend;
        public event Action<WebSocketConnection, OnReceiveEventargs> OnReceive;
        public event Action<WebSocketConnection, OnCloceEventargs> OnClose;
        public event Action<WebSocketConnection, OnErrorEventargs> OnError;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="configKey"></param>
        public WebSocketService(Core.Environment environment, string configKey = null)
        {
            var configuration = environment.GetConfiguration(configKey ?? "WebSocket");
            _config = configuration.GetConfig<Config>(configKey);
            if (_config == null) throw new Exception("未读取到WebSocket配置");
            _logger = ServiceHost.Provider.GetLogger<WebSocketService>();
            _connections = new Dictionary<string, WebSocketConnection>();
        }

        /// <summary>
        /// 开启服务
        /// </summary>
        /// <returns></returns>
        public Task OpenServiceAsync()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(builder =>
                {
                    builder.ConfigureLogging(builder => builder.AddFilter("Microsoft", LogLevel.Warning))
                    // wss证书路径
                    .UseContentRoot($"{AppDomain.CurrentDomain.BaseDirectory}{_config.ContentRoot}")
                    .UseKestrel(options =>
                    {
                        if (_config.Listen == null || _config.Listen.Count <= 0) throw new Exception("缺少Listen配置");
                        string listen = "";
                        // 监听地址及端口
                        if (_config.Listen.ContainsKey("ws"))
                        {
                            string[] ipAndPort = _config.Listen["ws"].Split(":");
                            var ip = string.IsNullOrEmpty(ipAndPort[0]) ? IPAddress.Any : IPAddress.Parse(ipAndPort[0]);
                            var port = int.Parse(ipAndPort[1]);
                            options.Listen(ip, port);
                            listen += $"ws://{ip}:{port}";
                        }
                        if (_config.Listen.ContainsKey("wss"))
                        {
                            string[] addrAndCert = _config.Listen["wss"].Split(";");
                            string[] ipAndPort = addrAndCert[0].Split(":");
                            var ip = string.IsNullOrEmpty(ipAndPort[0]) ? IPAddress.Any : IPAddress.Parse(ipAndPort[0]);
                            var port = int.Parse(ipAndPort[1]);
                            options.Listen(ip, port, listenOptions =>
                            {
                                string[] certAndPassword = addrAndCert[1].Split(",");
                                listenOptions.UseHttps(certAndPassword[0], certAndPassword[1]);
                            });
                            if (!string.IsNullOrEmpty(listen)) listen += ",";
                            listen += $"wss://{ip}:{port}";
                        }
                        _logger.LogDebug($"WebSocket服务已开启 => 监听地址:{listen}");
                    })
                    .Configure(app =>
                    {
                        app.UseWebSockets();
                        // 请求预处理
                        app.Use(async (context, next) =>
                        {
                            if (!context.WebSockets.IsWebSocketRequest)
                            {
                                await context.Response.WriteAsync(ResultInfo.Fail(message: "非WebSocket请求").ToJson());
                                return;
                            }
                            await next();
                        });
                        // 自定义处理
                        app.Run(async context =>
                        {
                            var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                            var connection = new WebSocketConnection(webSocket);
                            _connections[connection.ID] = connection;
                            OnConnect?.Invoke(connection, new OnConnectEventargs(context.Request.Headers));
                            var dataBytes = new List<byte>();
                            while (true)
                            {
                                try
                                {
                                    var buffer = new byte[_config.BufferSize];
                                    var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                                    if (result.CloseStatus.HasValue)
                                        break;
                                    dataBytes.AddRange(buffer.Take(result.Count));
                                    if (!result.EndOfMessage) continue;
                                    var msg = Encoding.UTF8.GetString(dataBytes.ToArray());
                                    OnReceive?.Invoke(connection, new OnReceiveEventargs(msg));
                                    dataBytes.Clear();
                                }
                                catch (Exception ex)
                                {
                                    OnError?.Invoke(connection, new OnErrorEventargs(ex));
                                    break;
                                }
                            }
                            OnClose?.Invoke(connection, new OnCloceEventargs() { });
                            connection.Dispose();
                            _connections.Remove(connection.ID);
                        });
                    });
                }).Build().RunAsync();
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
            var client = _connections[id];
            if (client.WebSocket.State != WebSocketState.Open)
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
                .ContinueWith(a => OnSend?.Invoke(connection, new OnSendEventargs(Encoding.UTF8.GetString(buffer))));
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
                foreach (var item in _connections.Values)
                {
                    item.CloseAsync();
                }
            });
        }
    }
}
