using System;
using System.IO;
using System.Net;
using System.Net.WebSockets;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Autofac;
using DwFramework.Core;

namespace DwFramework.WEB
{
    public static class WebHostExtension
    {
        /// <summary>
        /// 配置Web主机
        /// </summary>
        /// <param name="host"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureWebHost(this ServiceHost host, Action<IWebHostBuilder> configure)
        {
            host.ConfigureHostBuilder(builder => builder.ConfigureWebHost(configure));
            return host;
        }

        /// <summary>
        /// 配置WebApi
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="host"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureWebApi<T>(this ServiceHost host, Config config) where T : class
        {
            if (config == null) throw new Exception("未读取到WebAPI配置");
            if (config.Listens == null || config.Listens.Count <= 0) throw new Exception("缺少Listen配置");
            var webApiService = new WebApiService(host, config, typeof(T));
            host.ConfigureContainer(builder => builder.RegisterInstance(webApiService).SingleInstance());
            return host;
        }

        /// <summary>
        /// 配置WebApi
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="host"></param>
        /// <param name="configuration"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureWebApi<T>(this ServiceHost host, IConfiguration configuration, string path = null) where T : class
        {
            var config = configuration.GetConfig<Config>(path);
            host.ConfigureWebApi<T>(config);
            return host;
        }

        /// <summary>
        /// 配置WebApi
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="host"></param>
        /// <param name="file"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureWebApiWithJson<T>(this ServiceHost host, string file, string path = null) where T : class
            => host.ConfigureWebApi<T>(new ConfigurationBuilder().AddJsonFile(file).Build(), path);

        /// <summary>
        /// 配置WebApi
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="host"></param>
        /// <param name="stream"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureWebApiWithJson<T>(this ServiceHost host, Stream stream, string path = null) where T : class
            => host.ConfigureWebApi<T>(new ConfigurationBuilder().AddJsonStream(stream).Build(), path);

        /// <summary>
        /// 配置WebApi
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="host"></param>
        /// <param name="file"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureWebApiWithXml<T>(this ServiceHost host, string file, string path = null) where T : class
            => host.ConfigureWebApi<T>(new ConfigurationBuilder().AddXmlFile(file).Build(), path);

        /// <summary>
        /// 配置WebApi
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="host"></param>
        /// <param name="stream"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureWebApiWithXml<T>(this ServiceHost host, Stream stream, string path = null) where T : class
            => host.ConfigureWebApi<T>(new ConfigurationBuilder().AddXmlStream(stream).Build(), path);

        /// <summary>
        /// 配置WebSocket
        /// </summary>
        /// <param name="host"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureWebSocket(this ServiceHost host, Config config)
        {
            if (config == null) throw new Exception("未读取到WebSocket配置");
            if (config.Listens == null || config.Listens.Count <= 0) throw new Exception("缺少Listen配置");
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
                         // 请求预处理
                         //  app.Use(async (context, next) =>
                         //       {
                         //           if (!context.WebSockets.IsWebSocketRequest)
                         //           {
                         //               await context.Response.WriteAsync(ResultInfo.Create(StatusCode.ERROR, message: "非WebSocket请求").ToJson());
                         //               return;
                         //           }
                         //           await next();
                         //       });
                         //  // 开始接受连接
                         //  app.Run(async context =>
                         //       {
                         //           var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                         //           var connection = new WebSocketConnection(webSocket, _config.BufferSize, out var resetEvent)
                         //           {
                         //               OnClose = OnClose,
                         //               OnSend = OnSend,
                         //               OnReceive = OnReceive,
                         //               OnError = OnError
                         //           };
                         //           _connections[connection.ID] = connection;
                         //           OnConnect?.Invoke(connection, new OnConnectEventArgs() { Header = context.Request.Headers });
                         //           _ = connection.BeginReceiveAsync();
                         //           resetEvent.WaitOne();
                         //       });
                     });
                });
            });
            return host;
        }
    }
}
