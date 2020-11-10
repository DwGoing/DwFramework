using System;
using Autofac;

using DwFramework.Core;

namespace DwFramework.Rpc.Plugins
{
    public static class ClusterExtension
    {
        /// <summary>
        /// 注册集群服务
        /// </summary>
        /// <param name="host"></param>
        /// <param name="configFilePath"></param>
        /// <returns></returns>
        public static ServiceHost RegisterClusterImpl(this ServiceHost host, string configFilePath = null)
        {
            if (!string.IsNullOrEmpty(configFilePath))
            {
                host.AddJsonConfig(configFilePath, "Cluster");
                host.RegisterType<ClusterImpl>().SingleInstance();
            }
            else host.Register(c => new ClusterImpl(c.Resolve<Core.Environment>(), "Cluster")).SingleInstance();
            host.OnInitializing += provider => provider.().AddService(provider.GetClusterImpl());
            host.OnInitialized += provider => provider.GetClusterImpl().Init();
            return host;
        }

        /// <summary>
        /// 注册集群服务
        /// </summary>
        /// <param name="host"></param>
        /// <param name="linkUrl"></param>
        /// <param name="healthCheckPerMs"></param>
        /// <param name="bootPeer"></param>
        /// <returns></returns>
        public static ServiceHost RegisterClusterImpl(this ServiceHost host, string linkUrl, int healthCheckPerMs, string bootPeer = null)
        {
            var clusterImpl = new ClusterImpl(linkUrl, healthCheckPerMs, bootPeer);
            host.Register(context => clusterImpl).AsSelf().SingleInstance();
            host.OnInitializing += provider => provider.GetRpcService().AddService(clusterImpl);
            host.OnInitialized += _ => clusterImpl.Init();
            return host;
        }

        /// <summary>
        /// 获取集群服务
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static ClusterImpl GetClusterImpl(this IServiceProvider provider) => provider.GetService<ClusterImpl>();
    }
}
