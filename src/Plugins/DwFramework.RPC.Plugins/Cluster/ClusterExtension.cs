using System;
using Autofac;

using DwFramework.Core;

namespace DwFramework.RPC.Plugins
{
    public static class ClusterExtension
    {
        /// <summary>
        /// 注册集群服务
        /// </summary>
        /// <param name="host"></param>
        /// <param name="path"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static ServiceHost RegisterClusterImpl(this ServiceHost host, string path = null, string key = null)
        {
            host.Register(_ => new ClusterImpl(path, key)).SingleInstance();
            host.OnInitializing += provider => provider.GetRPCService().AddService(provider.GetClusterImpl());
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
            host.OnInitializing += provider => provider.GetRPCService().AddService(clusterImpl);
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
