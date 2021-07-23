using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net;
using System.Net.WebSockets;
using System.Reflection;
using System.IO.Compression;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProtoBuf.Grpc.Server;
using ProtoBuf.Grpc.Configuration;
using DwFramework.Core;

namespace DwFramework.Web
{
    public sealed class WebService
    {
        public static WebService Instance { get; private set; }

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
        /// <param name="configuration"></param>
        /// <param name="configureWebHostBuilder"></param>
        private WebService(ServiceHost host, IConfiguration configuration, Action<IWebHostBuilder> configureWebHostBuilder)
        {
            _config = configuration.Get<Config.Web>();
            if (_config == null) throw new NotFoundException("缺少Web配置");
            host.ConfigureHostBuilder(builder => builder.ConfigureWebHostDefaults(webHostBuilder =>
            {
                if (!string.IsNullOrEmpty(_config.ContentRoot)) webHostBuilder.UseContentRoot(_config.ContentRoot);
                webHostBuilder.UseKestrel(options =>
                {
                    foreach (var item in _config.Listens)
                    {
                        switch (item.Scheme)
                        {
                            case Scheme.Http:
                            case Scheme.Rpc:
                                options.Listen(string.IsNullOrEmpty(item.Ip) ? IPAddress.Any : IPAddress.Parse(item.Ip), item.Port, listenOptions =>
                                {
                                    if (item.Scheme == Scheme.Rpc) listenOptions.Protocols = HttpProtocols.Http2;
                                });
                                break;
                            case Scheme.Https:
                            case Scheme.Rpcs:
                                options.Listen(string.IsNullOrEmpty(item.Ip) ? IPAddress.Any : IPAddress.Parse(item.Ip), item.Port, listenOptions =>
                                {
                                    listenOptions.UseHttps(item.Cert, item.Password);
                                    if (item.Scheme == Scheme.Rpc) listenOptions.Protocols = HttpProtocols.Http2;
                                });
                                break;
                            default:
                                continue;
                        }
                    }
                });
                configureWebHostBuilder?.Invoke(webHostBuilder);
            }));
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="host"></param>
        /// <param name="configuration"></param>
        /// <param name="configureWebHostBuilder"></param>
        /// <returns></returns>
        public static WebService Init(ServiceHost host, IConfiguration configuration, Action<IWebHostBuilder> configureWebHostBuilder)
        {
            Instance = new WebService(host, configuration, configureWebHostBuilder);
            return Instance;
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

        /// <summary>
        /// 添加Rpc服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="rpcImpls"></param>
        public void AddRpcImplements(IServiceCollection services, params Type[] rpcImpls)
        {
            AddRpcImplementFromAssemblies();
            foreach (var item in rpcImpls) AddRpcImplement(item);
            services.AddCodeFirstGrpc(config => config.ResponseCompressionLevel = CompressionLevel.Optimal);
            services.AddSingleton(BinderConfiguration.Create(binder: new RpcServiceBinder(services)));
            services.AddCodeFirstGrpcReflection();
        }

        /// <summary>
        /// 匹配Rpc路由
        /// </summary>
        /// <param name="endpoints"></param>
        public void MapRpcImplements(IEndpointRouteBuilder endpoints)
        {
            _rpcImplBuild?.Invoke(endpoints);
            endpoints.MapCodeFirstGrpcReflectionService();
        }

        /// <summary>
        /// 使用WebSocket中间件
        /// </summary>
        /// <param name="app"></param>
        public void UseWebSocket(IApplicationBuilder app)
        {
            app.UseWebSockets();
            app.Use(async (context, next) =>
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
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
                    return;
                }
                await next();
            });
        }

        /// <summary>
        /// 获取连接
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public WebSocketConnection GetWebSocketConnection(string id)
        {
            if (!_webSocketConnections.ContainsKey(id)) return null;
            return _webSocketConnections[id];
        }

        /// <summary>
        /// 广播消息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public void BroadCast(byte[] data)
        {
            foreach (var item in _webSocketConnections.Values)
            {
                _ = item.SendAsync(data);
            }
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task CloseAsync(string id)
        {
            if (!_webSocketConnections.ContainsKey(id)) return Task.CompletedTask;
            var connection = _webSocketConnections[id];
            return connection.CloseAsync(WebSocketCloseStatus.NormalClosure);
        }
    }
}