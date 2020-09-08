﻿using Autofac;

using DwFramework.Core;
using DwFramework.Core.Extensions;
using DwFramework.Rpc.Extensions;

namespace DwFramework.Rpc.Plugins.Cluster
{
    public static class ClusterExtension
    {
        /// <summary>
        /// 注册集群服务
        /// </summary>
        /// <param name="host"></param>
        /// <param name="linkUrl"></param>
        /// <param name="healthCheckPerMs"></param>
        /// <param name="bootPeer"></param>
        /// <returns></returns>
        public static ServiceHost RegisterClusterImpl(this ServiceHost host, string linkUrl, int healthCheckPerMs = 10000, string bootPeer = null)
        {
            var clusterImpl = new ClusterImpl(linkUrl, healthCheckPerMs, bootPeer);
            host.Register(context => clusterImpl).AsSelf().SingleInstance();
            host.OnInitializing += provider => provider.GetRpcService().AddService(clusterImpl);
            return host;
        }
    }
}