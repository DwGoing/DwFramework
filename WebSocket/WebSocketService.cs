using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Autofac.Extensions.DependencyInjection;

using DwFramework.Core;
using DwFramework.Core.Models;
using DwFramework.Core.Extensions;

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
        public static async void InitWebSocketServiceAsync(this AutofacServiceProvider provider, Action<string, System.Net.WebSockets.WebSocket> handler)
        {
            await provider.GetService<IWebSocketService, WebSocketService>().OpenService(handler);
        }

        /// <summary>
        /// 请求预处理
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UsePreHandler(this IApplicationBuilder app)
        {
            return app.Use(async (context, next) =>
            {
                if (!context.WebSockets.IsWebSocketRequest)
                {
                    await context.Response.WriteAsync(ResultInfo.Fail("非WebSocket请求").ToJson());
                    return;
                }
                await next();
            });
        }

        /// <summary>
        /// 自定义处理
        /// </summary>
        /// <param name="app"></param>
        /// <param name="handler"></param>
        public static void RunCustomHandler(this IApplicationBuilder app, Action<string, System.Net.WebSockets.WebSocket> handler)
        {
            app.Run(async context =>
           {
               int MAX_BYTES_COUNT = 1024 * 4;
               var webSocket = await context.WebSockets.AcceptWebSocketAsync();
               var buffer = new byte[MAX_BYTES_COUNT];
               var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
               while (!result.CloseStatus.HasValue)
               {
                   try
                   {
                       var msg = System.Text.Encoding.UTF8.GetString(buffer, 0, result.Count);
                       handler?.Invoke(msg, webSocket);
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
           });
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

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="configuration"></param>
        public WebSocketService(IConfiguration configuration)
        {
            _configuration = configuration;
            _config = _configuration.GetSection("WebSocket").Get<Config>();
        }

        /// <summary>
        /// 开启WebSocket服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public Task OpenService(Action<string, System.Net.WebSockets.WebSocket> handler)
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
                        app.UsePreHandler();
                        app.RunCustomHandler(handler);
                    })
                    .Build();
                builder.Run();
            });
        }
    }
}
