using System;
using System.Collections.Generic;
using System.Net;
using System.IO.Compression;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using ProtoBuf.Grpc.Server;
using ProtoBuf.Grpc.Configuration;
using DwFramework.Core;

namespace DwFramework.Web
{
    public sealed class WebService
    {
        private readonly Config.Web _config;
        private event Action<IEndpointRouteBuilder> _rpcImplBuild;
        private readonly Dictionary<string, WebSocketConnection> _webSocketConnections = new();

        public event Action<WebSocketConnection, OnConnectEventArgs> OnWebSocketConnect;
        public event Action<WebSocketConnection, OnCloceEventArgs> OnWebSocketClose;
        public event Action<WebSocketConnection, OnSendEventArgs> OnWebSocketSend;
        public event Action<WebSocketConnection, OnReceiveEventArgs> OnWebSocketReceive;
        public event Action<WebSocketConnection, OnErrorEventArgs> OnWebSocketError;


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="host"></param>
        /// <param name="config"></param>
        public WebService(
            ServiceHost host, Config.Web config,
            Action<IServiceCollection> configureServices = null,
            Action<IApplicationBuilder> configure = null,
            Action<IEndpointRouteBuilder> configureEndpoints = null
        )
        {
            _config = config;
            if (config.EnableRpc) AddRpcImplementFromAssemblies();
            host.ConfigureWebHost(webHostBuilder =>
            {
                webHostBuilder.UseContentRoot($"{AppDomain.CurrentDomain.BaseDirectory}{_config.ContentRoot}")
                .UseKestrel(options =>
                {
                    if (config.EnableHttp)
                    {
                        if (config.Listens.ContainsKey("http"))
                        {
                            var ipAndPort = config.Listens["http"].Split(":");
                            var ip = string.IsNullOrEmpty(ipAndPort[0]) ? IPAddress.Any : IPAddress.Parse(ipAndPort[0]);
                            var port = int.Parse(ipAndPort[1]);
                            options.Listen(ip, port);
                        }
                        if (config.Listens.ContainsKey("https"))
                        {
                            var addrAndCert = config.Listens["https"].Split(";");
                            var ipAndPort = addrAndCert[0].Split(":");
                            var ip = string.IsNullOrEmpty(ipAndPort[0]) ? IPAddress.Any : IPAddress.Parse(ipAndPort[0]);
                            var port = int.Parse(ipAndPort[1]);
                            options.Listen(ip, port, listenOptions =>
                            {
                                var certAndPassword = addrAndCert[1].Split(",");
                                listenOptions.UseHttps(certAndPassword[0], certAndPassword[1]);
                            });
                        }
                    }
                    if (config.EnableRpc)
                    {
                        if (_config.Listens.ContainsKey("rpc"))
                        {
                            var ipAndPort = _config.Listens["rpc"].Split(":");
                            var ip = string.IsNullOrEmpty(ipAndPort[0]) ? IPAddress.Any : IPAddress.Parse(ipAndPort[0]);
                            var port = int.Parse(ipAndPort[1]);
                            options.Listen(ip, port, listenOptions =>
                            {
                                listenOptions.Protocols = HttpProtocols.Http2;
                            });
                        }
                        if (_config.Listens.ContainsKey("rpcs"))
                        {
                            var addrAndCert = _config.Listens["rpcs"].Split(";");
                            var ipAndPort = addrAndCert[0].Split(":");
                            var ip = string.IsNullOrEmpty(ipAndPort[0]) ? IPAddress.Any : IPAddress.Parse(ipAndPort[0]);
                            var port = int.Parse(ipAndPort[1]);
                            options.Listen(ip, port, listenOptions =>
                            {
                                var certAndPassword = addrAndCert[1].Split(",");
                                listenOptions.UseHttps(certAndPassword[0], certAndPassword[1]);
                                listenOptions.Protocols = HttpProtocols.Http2;
                            });
                        }
                    }
                    if (config.EnableWebSocket)
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
                    }
                })
                .ConfigureServices(services =>
                {
                    configureServices?.Invoke(services);
                    if (config.EnableRpc)
                    {
                        services.AddCodeFirstGrpc(config =>
                        {
                            config.ResponseCompressionLevel = CompressionLevel.Optimal;
                        });
                        services.AddSingleton(BinderConfiguration.Create(binder: new RpcServiceBinder(services)));
                        services.AddCodeFirstGrpcReflection();
                    }
                })
                .Configure(app =>
                {
                    configure?.Invoke(app);
                    app.UseRouting();
                    if (config.EnableHttp)
                    {

                    }
                    if (config.EnableRpc)
                    {

                    }
                    if (config.EnableWebSocket)
                    {
                        app.UseWebSockets();
                        // 接受连接
                        app.Run(async context =>
                        {
                            if (!context.WebSockets.IsWebSocketRequest) return;
                            var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                            if (webSocket == null) return;
                            var connection = new WebSocketConnection(webSocket, _config.BufferSize, out var resetEvent)
                            {
                                OnClose = OnWebSocketClose,
                                OnSend = OnWebSocketSend,
                                OnReceive = OnWebSocketReceive,
                                OnError = OnWebSocketError
                            };
                            _webSocketConnections[connection.ID] = connection;
                            OnWebSocketConnect?.Invoke(connection, new OnConnectEventArgs() { Header = context.Request.Headers });
                            _ = connection.BeginReceiveAsync();
                            resetEvent.WaitOne();
                        });
                    }
                    app.UseEndpoints(endpoints =>
                    {
                        configureEndpoints?.Invoke(endpoints);
                        if (config.EnableRpc)
                        {
                            _rpcImplBuild?.Invoke(endpoints);
                            endpoints.MapCodeFirstGrpcReflectionService();
                        }
                    });
                });
            });
        }

        /// <summary>
        /// 添加RPC服务
        /// </summary>
        /// <param name="type"></param>
        private void AddRpcImplement(Type type)
        {
            var method = typeof(GrpcEndpointRouteBuilderExtensions).GetMethod("MapGrpcService");
            var genericMethod = method.MakeGenericMethod(type);
            _rpcImplBuild += endpoint => genericMethod.Invoke(null, new object[] { endpoint });
        }

        /// <summary>
        /// 从程序集中注册Rpc服务
        /// </summary>
        private void AddRpcImplementFromAssemblies()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    var attribute = type.GetCustomAttribute<RPCAttribute>();
                    if (attribute == null) continue;
                    AddRpcImplement(type);
                }
            }
        }
    }
}