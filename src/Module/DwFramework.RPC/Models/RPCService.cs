using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using ProtoBuf.Grpc.Configuration;
using ProtoBuf.Grpc.Server;

using DwFramework.Core;
using DwFramework.Core.Plugins;
using DwFramework.Core.Extensions;

namespace DwFramework.RPC
{
    public sealed class Config
    {
        public string ContentRoot { get; set; }
        public Dictionary<string, string> Listen { get; set; }
    }

    public sealed class RPCService
    {
        private readonly Config _config;
        private readonly ILogger<RPCService> _logger;
        private event Action<IServiceCollection> _onConfigureServices;
        private event Action<IEndpointRouteBuilder> _onEndpointsBuild;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="configKey"></param>
        /// <param name="configPath"></param>
        public RPCService(string configKey = null, string configPath = null, ILogger<RPCService> logger = null)
        {
            _config = ServiceHost.Environment.GetConfiguration<Config>(configKey, configPath);
            if (_config == null) throw new Exception("RPC初始化异常 => 未读取到Rpc配置");
            _logger = logger;
        }

        /// <summary>
        /// 添加内部服务
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public RPCService AddInternalService(Action<IServiceCollection> action)
        {
            _onConfigureServices += action;
            return this;
        }

        /// <summary>
        /// 添加外部服务
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public RPCService AddExternalService(Type type)
        {
            _onConfigureServices += services => services.AddTransient(type, _ => ServiceHost.Provider.GetService(type));
            return this;
        }

        /// <summary>
        /// 添加外部服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public RPCService AddExternalService<T>() where T : class
        {
            AddExternalService(typeof(T));
            return this;
        }

        /// <summary>
        /// 添加RPC服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public RPCService AddRpcImplement<T>() where T : class
        {
            _onEndpointsBuild += endpoint => endpoint.MapGrpcService<T>();
            return this;
        }

        /// <summary>
        /// 添加RPC服务
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public RPCService AddRpcImplement(Type type)
        {
            var method = typeof(GrpcEndpointRouteBuilderExtensions).GetMethod("MapGrpcService");
            var genericMethod = method.MakeGenericMethod(type);
            _onEndpointsBuild += endpoint => genericMethod.Invoke(null, new object[] { endpoint });
            return this;
        }

        /// <summary>
        /// 从程序集中注册Rpc函数
        /// </summary>
        private void RegisterFuncFromAssemblies()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                types.ForEach(type =>
                {
                    var attribute = type.GetCustomAttribute<RPCAttribute>();
                    if (attribute == null) return;
                    AddInternalService(services => services.AddTransient(type));
                    AddRpcImplement(type);
                    attribute.ExternalServices.ForEach(item => AddExternalService(item));
                });
            }
        }

        /// <summary>
        /// 开启服务
        /// </summary>
        /// <returns></returns>
        public async Task OpenServiceAsync()
        {
            RegisterFuncFromAssemblies();
            var builder = Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(builder =>
                {
                    builder.ConfigureLogging(builder => builder.AddFilter("Microsoft", LogLevel.Warning))
                    // https证书路径
                    .UseContentRoot($"{AppDomain.CurrentDomain.BaseDirectory}{_config.ContentRoot}")
                    .UseKestrel(options =>
                    {
                        if (_config.Listen == null || _config.Listen.Count <= 0) throw new Exception("缺少Listen配置");
                        string listen = "";
                        // 监听地址及端口
                        if (_config.Listen.ContainsKey("http"))
                        {
                            string[] ipAndPort = _config.Listen["http"].Split(":");
                            var ip = string.IsNullOrEmpty(ipAndPort[0]) ? IPAddress.Any : IPAddress.Parse(ipAndPort[0]);
                            var port = int.Parse(ipAndPort[1]);
                            options.Listen(ip, port, listenOptions =>
                            {
                                listenOptions.Protocols = HttpProtocols.Http2;
                            });
                            listen += $"http://{ip}:{port}";
                        }
                        if (_config.Listen.ContainsKey("https"))
                        {
                            string[] addrAndCert = _config.Listen["https"].Split(";");
                            string[] ipAndPort = addrAndCert[0].Split(":");
                            var ip = string.IsNullOrEmpty(ipAndPort[0]) ? IPAddress.Any : IPAddress.Parse(ipAndPort[0]);
                            var port = int.Parse(ipAndPort[1]);
                            options.Listen(ip, port, listenOptions =>
                            {
                                string[] certAndPassword = addrAndCert[1].Split(",");
                                listenOptions.UseHttps(certAndPassword[0], certAndPassword[1]);
                                listenOptions.Protocols = HttpProtocols.Http2;
                            });
                            if (!string.IsNullOrEmpty(listen)) listen += ",";
                            listen += $"https://{ip}:{port}";
                        }
                        _logger?.LogInformationAsync($"RPC服务已开启 => 监听地址:{listen}");
                    })
                    .ConfigureServices(services =>
                    {
                        services.AddCodeFirstGrpc(config =>
                        {
                            config.ResponseCompressionLevel = System.IO.Compression.CompressionLevel.Optimal;
                        });
                        services.TryAddSingleton(BinderConfiguration.Create(binder: new ServiceBinderWithServiceResolutionFromServiceCollection(services)));
                        services.AddCodeFirstGrpcReflection();
                        _onConfigureServices?.Invoke(services);
                    })
                    .Configure(app =>
                    {
                        app.UseRouting();
                        app.UseEndpoints(endpoints =>
                        {
                            _onEndpointsBuild?.Invoke(endpoints);
                            endpoints.MapCodeFirstGrpcReflectionService();
                        });
                    });
                });
            await builder.Build().RunAsync();
        }
    }
}