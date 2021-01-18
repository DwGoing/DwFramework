using System;
using System.Threading;
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
            public IHeaderDictionary Header { get; }

            public OnConnectEventargs(IHeaderDictionary header)
            {
                Header = header;
            }
        }

        public class OnSendEventargs : EventArgs
        {
            public byte[] Data { get; }

            public OnSendEventargs(byte[] data)
            {
                Data = data;
            }
        }

        public class OnReceiveEventargs : EventArgs
        {
            public byte[] Data { get; }

            public OnReceiveEventargs(byte[] data)
            {
                Data = data;
            }
        }

        public class OnCloceEventargs : EventArgs
        {
            public WebSocketCloseStatus? CloseStatus { get; }

            public OnCloceEventargs(WebSocketCloseStatus? closeStatus)
            {
                CloseStatus = closeStatus;
            }
        }

        public class OnErrorEventargs : EventArgs
        {
            public Exception Exception { get; }

            public OnErrorEventargs(Exception exception)
            {
                Exception = exception;
            }
        }

        private readonly Config _config;
        private readonly ILogger<WebSocketService> _logger;
        private readonly Dictionary<string, WebSocketConnection> _connections;

        public event Action<WebSocketConnection, OnConnectEventargs> OnConnect;
        public event Action<WebSocketConnection, OnCloceEventargs> OnClose;
        public event Action<WebSocketConnection, OnSendEventargs> OnSend;
        public event Action<WebSocketConnection, OnReceiveEventargs> OnReceive;
        public event Action<WebSocketConnection, OnErrorEventargs> OnError;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="configKey"></param>
        /// <param name="configPath"></param>
        public WebSocketService(string configKey = null, string configPath = null)
        {
            _config = ServiceHost.Environment.GetConfiguration<Config>(configKey, configPath);
            if (_config == null) throw new Exception("未读取到WebSocket配置");
            _logger = ServiceHost.Provider.GetLogger<WebSocketService>();
            _connections = new Dictionary<string, WebSocketConnection>();
        }

        /// <summary>
        /// 开启服务
        /// </summary>
        /// <returns></returns>
        public async Task OpenServiceAsync()
        {
            await Host.CreateDefaultBuilder().ConfigureWebHostDefaults(builder =>
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
                    _logger?.LogInformationAsync($"WebSocket服务已开启 => 监听地址:{listen}");
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
                    var buffer = new byte[_config.BufferSize];
                    var dataBytes = new List<byte>();
                    WebSocketCloseStatus? closeStates = null;
                    while (true)
                    {
                        try
                        {
                            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                            if (result.CloseStatus.HasValue)
                            {
                                closeStates = result.CloseStatus;
                                break;
                            }
                            dataBytes.AddRange(buffer.Take(result.Count));
                            if (!result.EndOfMessage) continue;
                            OnReceive?.Invoke(connection, new OnReceiveEventargs(dataBytes.ToArray()));
                            dataBytes.Clear();
                        }
                        catch (Exception ex)
                        {
                            OnError?.Invoke(connection, new OnErrorEventargs(ex));
                            continue;
                        }
                    }
                    OnClose?.Invoke(connection, new OnCloceEventargs(closeStates));
                    if (connection.WebSocket.State == WebSocketState.CloseReceived)
                        await connection.CloseAsync(WebSocketCloseStatus.NormalClosure);
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
        public async Task SendAsync(string id, byte[] buffer)
        {
            RequireClient(id);
            var connection = _connections[id];
            await connection.SendAsync(buffer).ContinueWith(task =>
            {
                if (task.IsCompletedSuccessfully) OnSend?.Invoke(connection, new OnSendEventargs(buffer));
                else OnError?.Invoke(connection, new OnErrorEventargs(task.Exception.InnerException));
            });
        }

        /// <summary>
        /// 广播消息
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public async Task BroadCastAsync(byte[] buffer)
        {
            foreach (var item in _connections.Values)
            {
                await SendAsync(item.ID, buffer);
            }
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task CloseAsync(string id, WebSocketCloseStatus closeStatus)
        {
            RequireClient(id);
            var connection = _connections[id];
            await connection.CloseAsync(closeStatus);
        }

        /// <summary>
        /// 断开所有连接
        /// </summary>
        /// <returns></returns>
        public async Task CloseAllAsync(WebSocketCloseStatus closeStatus)
        {
            foreach (var item in _connections.Values)
            {
                await item.CloseAsync(closeStatus);
            }
        }
    }
}
