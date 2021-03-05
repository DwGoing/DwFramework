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
        public static ServiceHost RegisterCluster(this ServiceHost host, string configKey = null, string configPath = null)
        {
            host.Register(c => new Cluster(configKey, configPath, c.Resolve<ILogger<Cluster>>())).SingleInstance();
            host.OnInitializing += provider => provider.GetRPCService().AddService<Cluster>();
            host.OnInitialized += async provider => await provider.GetCluster().InitAsync(configKey, configPath);
            return host;
        }

        /// <summary>
        /// 获取集群服务
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static Cluster GetCluster(this IServiceProvider provider) => provider.GetService<Cluster>();
    }
}
