using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;

using DwFramework.Core;
using DwFramework.Core.Plugins;
using ProtoBuf.Grpc.Server;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ProtoBuf.Grpc.Configuration;

namespace DwFramework.RPC
{
    public sealed class RPCService : ConfigableService
    {
        public sealed class Config
        {
            public string ContentRoot { get; init; }
            public Dictionary<string, string> Listen { get; init; }
        }

        private readonly ILogger<RPCService> _logger;
        private Config _config;
        private CancellationTokenSource _cancellationTokenSource;
        private event Action<IServiceCollection> _onConfigureServices;
        private event Action<IEndpointRouteBuilder> _onEndpointsBuild;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger"></param>
        public RPCService(ILogger<RPCService> logger = null)
        {
            _logger = logger;
        }

        /// <summary>
        /// 读取配置
        /// </summary>
        /// <param name="path"></param>
        /// <param name="key"></param>
        public void ReadConfig(string path = null, string key = null)
        {
            try
            {
                _config = ReadConfig<Config>(path, key);
                if (_config == null) throw new Exception("未读取到Rpc配置");
            }
            catch (Exception ex)
            {
                _ = _logger?.LogErrorAsync(ex.Message);
                throw;
            }
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
            _onConfigureServices += services => services.AddSingleton(type, _ => ServiceHost.Provider.GetService(type));
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
        //private void RegisterFuncFromAssemblies()
        //{
        //    var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        //    assemblies.ForEach(assembly =>
        //    {
        //        var types = assembly.GetTypes();
        //        types.ForEach(type =>
        //        {
        //            var attr = type.GetCustomAttribute<RPCAttribute>();
        //            if (attr == null) return;
        //            var service = ServiceHost.Provider.GetService(type);
        //            if (service == null) return;
        //            _server.Services.Add(GetServerServiceDefinition(service));
        //        });
        //    });
        //}

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
                var builder = Host.CreateDefaultBuilder()
                    .ConfigureWebHostDefaults(builder =>
                    {
                        builder.ConfigureLogging(builder => builder.AddFilter("Microsoft", LogLevel.Warning))
                        // https证书路径
                        .UseContentRoot($"{AppDomain.CurrentDomain.BaseDirectory}{_config.ContentRoot}")
                        .UseKestrel(async options =>
                        {
                            if (_config.Listen == null || _config.Listen.Count <= 0) throw new Exception("缺少Listen配置");
                            var listen = "";
                            // 监听地址及端口
                            if (_config.Listen.ContainsKey("http"))
                            {
                                var ipAndPort = _config.Listen["http"].Split(":");
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
                                var addrAndCert = _config.Listen["https"].Split(";");
                                var ipAndPort = addrAndCert[0].Split(":");
                                var ip = string.IsNullOrEmpty(ipAndPort[0]) ? IPAddress.Any : IPAddress.Parse(ipAndPort[0]);
                                var port = int.Parse(ipAndPort[1]);
                                options.Listen(ip, port, listenOptions =>
                                {
                                    var certAndPassword = addrAndCert[1].Split(",");
                                    listenOptions.UseHttps(certAndPassword[0], certAndPassword[1]);
                                    listenOptions.Protocols = HttpProtocols.Http2;
                                });
                                if (!string.IsNullOrEmpty(listen)) listen += ",";
                                listen += $"https://{ip}:{port}";
                            }
                            if (_logger != null) await _logger?.LogInformationAsync($"RPC服务正在监听:{listen}");
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
        }
    }
}