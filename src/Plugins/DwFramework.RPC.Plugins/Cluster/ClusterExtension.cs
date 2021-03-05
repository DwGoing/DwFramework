using System;
using Autofac;
using Microsoft.Extensions.Logging;

using DwFramework.Core;

namespace DwFramework.RPC.Plugins
{
    public static class ClusterExtension
    {
        /// <summary>
        /// 注册集群服务
        /// </summary>
        /// <param name="host"></param>
        /// <param name="configKey"></param>
        /// <param name="configPath"></param>
        /// <returns></returns>
        public static ServiceHost RegisterClusterService(this ServiceHost host, string configKey = null, string configPath = null)
        {
            host.Register(c => new ClusterService(configKey, configPath, c.Resolve<ILogger<ClusterService>>())).SingleInstance();
            host.OnInitializing += provider =>
            {
                var service = provider.GetRPCService();
                service.AddExternalService<ClusterService>();
                service.AddRpcImplement<Cluster>();
            };
            host.OnInitialized += provider => provider.GetClusterService().Init();
            return host;
        }

        /// <summary>
        /// 获取集群服务
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static ClusterService GetClusterService(this IServiceProvider provider) => provider.GetService<ClusterService>();
    }
}
