using System;
using System.Collections.Generic;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

using DwFramework.Core;
using DwFramework.Core.Entities;
using DwFramework.Core.Plugins;
using DwFramework.Core.Extensions;

namespace DwFramework.WebSocket
{
    public sealed class WebSocketService : ConfigableServiceBase
    {
        public sealed class Config
        {
            public string ContentRoot { get; init; }
            public Dictionary<string, string> Listen { get; init; }
            public int BufferSize { get; init; } = 1024 * 4;
        }

        public class OnConnectEventArgs : EventArgs
        {
            public IHeaderDictionary Header { get; init; }
        }

        public class OnCloceEventArgs : EventArgs
        {
            public WebSocketCloseStatus? CloseStatus { get; init; }
        }

        public class OnSendEventArgs : EventArgs
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

        private readonly ILogger<WebSocketService> _logger;
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
        /// <param name="logger"></param>
        public WebSocketService(ILogger<WebSocketService> logger = null)
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
                if (_config == null) throw new Exception("未读取到WebSocket配置");
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
        /// 添加内部服务
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public WebSocketService AddInternalService(Action<IServiceCollection> action)
        {
            _onConfigureServices += action;
            return this;
        }

        /// <summary>
        /// 添加外部服务
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public WebSocketService AddExternalService(Type type)
        {
            _onConfigureServices += services => services.AddTransient(type, _ => ServiceHost.Provider.GetService(type));
            return this;
        }

        /// <summary>
        /// 添加外部服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public WebSocketService AddExternalService<T>() where T : class
        {
            AddExternalService(typeof(T));
            return this;
        }

        /// <summary>
        /// 运行服务
        /// </summary>
        /// <returns></returns>
        public async Task RunAsync()
        {
            try
            {
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = new CancellationTokenSource();
                var builder = Host.CreateDefaultBuilder().ConfigureWebHostDefaults(builder =>
                 {
                     builder.ConfigureLogging(builder => builder.AddFilter("Microsoft", LogLevel.Warning))
                     // wss证书路径
                     .UseContentRoot($"{AppDomain.CurrentDomain.BaseDirectory}{_config.ContentRoot}")
                     .UseKestrel(options =>
                     {
                         if (_config.Listen == null || _config.Listen.Count <= 0) throw new Exception("缺少Listen配置");
                         var listen = "";
                         // 监听地址及端口
                         if (_config.Listen.ContainsKey("ws"))
                         {
                             var ipAndPort = _config.Listen["ws"].Split(":");
                             var ip = string.IsNullOrEmpty(ipAndPort[0]) ? IPAddress.Any : IPAddress.Parse(ipAndPort[0]);
                             var port = int.Parse(ipAndPort[1]);
                             options.Listen(ip, port);
                             listen += $"ws://{ip}:{port}";
                         }
                         if (_config.Listen.ContainsKey("wss"))
                         {
                             var addrAndCert = _config.Listen["wss"].Split(";");
                             var ipAndPort = addrAndCert[0].Split(":");
                             var ip = string.IsNullOrEmpty(ipAndPort[0]) ? IPAddress.Any : IPAddress.Parse(ipAndPort[0]);
                             var port = int.Parse(ipAndPort[1]);
                             options.Listen(ip, port, listenOptions =>
                             {
                                 var certAndPassword = addrAndCert[1].Split(",");
                                 listenOptions.UseHttps(certAndPassword[0], certAndPassword[1]);
                             });
                             if (!string.IsNullOrEmpty(listen)) listen += ",";
                             listen += $"wss://{ip}:{port}";
                         }
                         _logger?.LogInformationAsync($"WebSocket服务正在监听:{listen}");
                     })
                     .ConfigureServices(services => _onConfigureServices?.Invoke(services))
                     .Configure(app =>
                     {
                         app.UseWebSockets();
                         // 请求预处理
                         app.Use(async (context, next) =>
                              {
                                  if (!context.WebSockets.IsWebSocketRequest)
                                  {
                                      await context.Response.WriteAsync(ResultInfo.Create(StatusCode.ERROR, message: "非WebSocket请求").ToJson());
                                      return;
                                  }
                                  await next();
                              });
                         // 开始接受连接
                         app.Run(async context =>
                              {
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
                await builder.Build().RunAsync(_cancellationTokenSource.Token);
            }
            catch (Exception ex)
            {
                _ = _logger?.LogErrorAsync(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        public void Stop()
        {
            _cancellationTokenSource.Cancel();
            _connections.Clear();
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
            _connections.Values.ForEach(async item => await item.CloseAsync(WebSocketCloseStatus.NormalClosure), (connection, ex) =>
            {
                OnError?.Invoke(connection, new OnErrorEventArgs() { Exception = ex });
            });
        }
    }
}
