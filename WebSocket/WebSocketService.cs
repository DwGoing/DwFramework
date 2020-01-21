using System;
using System.Collections.Generic;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using System.Text;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Autofac.Extensions.DependencyInjection;

using DwFramework.Core;
using DwFramework.Core.Models;
using DwFramework.Core.Extensions;
using DwFramework.WebSocket.Models;

namespace DwFramework.WebSocket
{
    public static class WebSocketServiceExtension
    {
        /// <summary>
        /// 注册WebSocket服务
        /// </summary>
        /// <param name="host"></param>
        public static void RegisterWebSocketService(this ServiceHost host)
        {
            host.RegisterType<IWebSocketService, WebSocketService>().SingleInstance();
        }

        /// <summary>
        /// 初始化WebSocket服务
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="handler"></param>
        public static async void InitWebSocketServiceAsync(this AutofacServiceProvider provider, OnConnectHandler onConnect = null, OnSendHandler onSend = null, OnReceiveHandler onReceive = null, OnCloseHandler onClose = null)
        {
            var service = provider.GetService<IWebSocketService, WebSocketService>();
            if (onConnect != null) service.OnConnect += onConnect;
            if (onSend != null) service.OnSend += onSend;
            if (onReceive != null) service.OnReceive += onReceive;
            if (onClose != null) service.OnClose += onClose;
            await service.OpenService();
        }
    }

    public class WebSocketService : IWebSocketService
    {
        public class Config
        {
            public string ContentRoot { get; set; }
            public Dictionary<string, string> Listen { get; set; }
        }

        private readonly IConfiguration _configuration;
        private readonly Config _config;
        private Dictionary<string, WebSocketClient> _clients;

        public event OnConnectHandler OnConnect;
        public event OnSendHandler OnSend;
        public event OnReceiveHandler OnReceive;
        public event OnCloseHandler OnClose;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="configuration"></param>
        public WebSocketService(IConfiguration configuration)
        {
            _configuration = configuration;
            _config = _configuration.GetSection("WebSocket").Get<Config>();
            _clients = new Dictionary<string, WebSocketClient>();
        }

        /// <summary>
        /// 开启WebSocket服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public Task OpenService()
        {
            return Task.Run(() =>
            {
                var builder = new WebHostBuilder()
                    // wss证书路径
                    .UseContentRoot($"{AppDomain.CurrentDomain.BaseDirectory}{_config.ContentRoot}")
                    .UseKestrel(options =>
                    {
                        // 监听地址及端口
                        if (_config.Listen == null || _config.Listen.Count <= 0)
                            options.Listen(IPAddress.Parse("0.0.0.0"), 5088);
                        else
                        {
                            if (_config.Listen.ContainsKey("ws"))
                            {
                                string[] ipAndPort = _config.Listen["ws"].Split(":");
                                options.Listen(IPAddress.Parse(ipAndPort[0]), int.Parse(ipAndPort[1]));
                            }
                            if (_config.Listen.ContainsKey("wss"))
                            {
                                string[] addrAndCert = _config.Listen["wss"].Split(";");
                                string[] ipAndPort = addrAndCert[0].Split(":");
                                options.Listen(IPAddress.Parse(ipAndPort[0]), int.Parse(ipAndPort[1]), listenOptions =>
                                {
                                    string[] certAndPassword = addrAndCert[1].Split(",");
                                    listenOptions.UseHttps(certAndPassword[0], certAndPassword[1]);
                                });
                            }
                        }
                    })
                    .Configure(app =>
                    {
                        app.UseWebSockets();
                        // 请求预处理
                        app.Use(async (context, next) =>
                        {
                            if (!context.WebSockets.IsWebSocketRequest)
                            {
                                await context.Response.WriteAsync(ResultInfo.Fail("非WebSocket请求").ToJson());
                                return;
                            }
                            await next();
                        });
                        // 自定义处理
                        app.Run(async context =>
                        {
                            int MAX_BYTES_COUNT = 1024 * 4;
                            var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                            var client = new WebSocketClient(webSocket);
                            _clients[client.ID] = client;
                            OnConnect?.Invoke(client, new OnConnectEventargs() { });
                            var buffer = new byte[MAX_BYTES_COUNT];
                            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                            while (!result.CloseStatus.HasValue)
                            {
                                try
                                {
                                    var msg = Encoding.UTF8.GetString(buffer, 0, result.Count);
                                    OnReceive?.Invoke(client, new OnReceiveEventargs(msg));
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                                finally
                                {
                                    buffer = new byte[MAX_BYTES_COUNT];
                                    result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                                }
                            }
                            OnClose?.Invoke(client, new OnCloceEventargs() { });
                        });
                    })
                    .Build();
                builder.Run();
            });
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public Task SendAsync(string id, byte[] buffer)
        {
            if (!_clients.ContainsKey(id))
                throw new Exception("该客户端不存在");
            var client = _clients[id];
            if (client.WebSocket.State != WebSocketState.Open)
                throw new Exception("发送失败");
            return client.SendAsync(buffer)
                .ContinueWith(a => OnSend?.Invoke(client, new OnSendEventargs(Encoding.UTF8.GetString(buffer)) { }));
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
        public Task BroadCast(string msg)
        {
            return Task.Run(() =>
            {
                byte[] buffer = Encoding.UTF8.GetBytes(msg);
                foreach (var item in _clients.Values)
                {
                    SendAsync(item.ID, buffer);
                }
            });
        }
    }
}
