using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Autofac;
using DwFramework.Core;

namespace DwFramework.Web.Rpc
{
    public static class RpcExtension
    {
        /// <summary>
        /// 配置Rpc
        /// </summary>
        public static ServiceHost ConfigureRpc(this ServiceHost host, Config config, params Type[] rpcImpls)
        {
            if (config == null) throw new Exception("未读取到Rpc配置");
            var rpcService = new RpcService(host, config, rpcImpls);
            host.ConfigureContainer(builder => builder.RegisterInstance(rpcService).SingleInstance());
            return host;
        }

        /// <summary>
        /// 配置Rpc
        /// </summary>
        /// <param name="host"></param>
        /// <param name="configuration"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureRpc(this ServiceHost host, IConfiguration configuration, string path = null, params Type[] rpcImpls)
        {
            var config = configuration.GetConfig<Config>(path);
            host.ConfigureRpc(config, rpcImpls);
            return host;
        }

        /// <summary>
        /// 配置Rpc
        /// </summary>
        /// <param name="host"></param>
        /// <param name="file"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureRpcWithJson(this ServiceHost host, string file, string path = null, params Type[] rpcImpls)
            => host.ConfigureRpc(new ConfigurationBuilder().AddJsonFile(file).Build(), path, rpcImpls);

        /// <summary>
        /// 配置Rpc
        /// </summary>
        /// <param name="host"></param>
        /// <param name="stream"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureRpcWithJson(this ServiceHost host, Stream stream, string path = null, params Type[] rpcImpls)
            => host.ConfigureRpc(new ConfigurationBuilder().AddJsonStream(stream).Build(), path, rpcImpls);

        /// <summary>
        /// 配置Rpc
        /// </summary>
        /// <param name="host"></param>
        /// <param name="file"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureRpcWithXml(this ServiceHost host, string file, string path = null, params Type[] rpcImpls)
            => host.ConfigureRpc(new ConfigurationBuilder().AddXmlFile(file).Build(), path, rpcImpls);

        /// <summary>
        /// 配置Rpc
        /// </summary>
        /// <param name="host"></param>
        /// <param name="stream"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ServiceHost ConfigureRpcWithXml(this ServiceHost host, Stream stream, string path = null, params Type[] rpcImpls)
            => host.ConfigureRpc(new ConfigurationBuilder().AddXmlStream(stream).Build(), path, rpcImpls);

        /// <summary>
        /// 获取Rpc服务
        /// </summary>
        /// <param name="provider"></param>
        /// <typeparam name="RpcService"></typeparam>
        /// <returns></returns>
        public static RpcService GetRpc(this IServiceProvider provider) => provider.GetService<RpcService>();
    }
}
