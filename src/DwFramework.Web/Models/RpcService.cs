// using System;
// using System.Net;
// using System.IO.Compression;
// using System.Reflection;
// using Microsoft.Extensions.Logging;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.AspNetCore.Routing;
// using Microsoft.AspNetCore.Builder;
// using Microsoft.Extensions.Hosting;
// using Microsoft.AspNetCore.Hosting;
// using Microsoft.AspNetCore.Server.Kestrel.Core;
// using ProtoBuf.Grpc.Server;
// using ProtoBuf.Grpc.Configuration;
// using DwFramework.Core;

// namespace DwFramework.Web.Rpc
// {
//     public sealed class RpcService
//     {
//         private readonly Config _config;
//         private event Action<IEndpointRouteBuilder> _onEndpointsBuild;

//         /// <summary>
//         /// 构造函数
//         /// </summary>
//         /// <param name="host"></param>
//         /// <param name="config"></param>
//         public RpcService(ServiceHost host, Config config, params Type[] rpcImpls)
//         {
//             _config = config;
//             foreach (var item in rpcImpls) AddRpcImplement(item);
//             AddRpcImplementFromAssemblies();
//             host.ConfigureHostBuilder(hostBuilder =>
//             {
//                 hostBuilder.ConfigureWebHost(webHostBuilder =>
//                 {
//                     webHostBuilder.UseContentRoot($"{AppDomain.CurrentDomain.BaseDirectory}{_config.ContentRoot}")
//                     .UseKestrel(options =>
//                     {
//                         if (_config.Listens.ContainsKey("rpc"))
//                         {
//                             var ipAndPort = _config.Listens["rpc"].Split(":");
//                             var ip = string.IsNullOrEmpty(ipAndPort[0]) ? IPAddress.Any : IPAddress.Parse(ipAndPort[0]);
//                             var port = int.Parse(ipAndPort[1]);
//                             options.Listen(ip, port, listenOptions =>
//                             {
//                                 listenOptions.Protocols = HttpProtocols.Http2;
//                             });
//                         }
//                         if (_config.Listens.ContainsKey("rpcs"))
//                         {
//                             var addrAndCert = _config.Listens["rpcs"].Split(";");
//                             var ipAndPort = addrAndCert[0].Split(":");
//                             var ip = string.IsNullOrEmpty(ipAndPort[0]) ? IPAddress.Any : IPAddress.Parse(ipAndPort[0]);
//                             var port = int.Parse(ipAndPort[1]);
//                             options.Listen(ip, port, listenOptions =>
//                             {
//                                 var certAndPassword = addrAndCert[1].Split(",");
//                                 listenOptions.UseHttps(certAndPassword[0], certAndPassword[1]);
//                                 listenOptions.Protocols = HttpProtocols.Http2;
//                             });
//                         }
//                     })
//                     .ConfigureServices(services =>
//                     {
//                         services.AddCodeFirstGrpc(config =>
//                         {
//                             config.ResponseCompressionLevel = CompressionLevel.Optimal;
//                         });
//                         services.AddSingleton(BinderConfiguration.Create(binder: new RpcServiceBinder(services)));
//                         services.AddCodeFirstGrpcReflection();
//                     })
//                     .Configure(app =>
//                     {
//                         app.UseRouting();
//                         app.UseEndpoints(endpoints =>
//                         {
//                             _onEndpointsBuild?.Invoke(endpoints);
//                             endpoints.MapCodeFirstGrpcReflectionService();
//                         });
//                     });
//                 });
//             });
//         }

//         /// <summary>
//         /// 添加RPC服务
//         /// </summary>
//         /// <param name="type"></param>
//         private void AddRpcImplement(Type type)
//         {
//             var method = typeof(GrpcEndpointRouteBuilderExtensions).GetMethod("MapGrpcService");
//             var genericMethod = method.MakeGenericMethod(type);
//             _onEndpointsBuild += endpoint => genericMethod.Invoke(null, new object[] { endpoint });
//         }

//         /// <summary>
//         /// 从程序集中注册Rpc服务
//         /// </summary>
//         private void AddRpcImplementFromAssemblies()
//         {
//             var assemblies = AppDomain.CurrentDomain.GetAssemblies();
//             foreach (var assembly in assemblies)
//             {
//                 var types = assembly.GetTypes();
//                 foreach (var type in types)
//                 {
//                     var attribute = type.GetCustomAttribute<RPCAttribute>();
//                     if (attribute == null) continue;
//                     AddRpcImplement(type);
//                 }
//             }
//         }
//     }
// }