using System;

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
        public static ServiceHost RegisterClusterService(this ServiceHost host, string path = null, string key = null)
        {
            host.RegisterType<ClusterService>().SingleInstance();
            host.OnInitializing += provider =>
            {
                var service = provider.GetRPCService();
                service.AddExternalService<ClusterService>();
                service.AddRpcImplement<Cluster>();
            };
            host.OnInitialized += provider => provider.RunClusterService(path, key);
            return host;
        }

        /// <summary>
        /// 获取集群服务
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static ClusterService GetClusterService(this IServiceProvider provider)
        {
            return provider.GetService<ClusterService>();
        }

        /// <summary>
        /// 运行服务
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="path"></param>
        /// <param name="key"></param>
        public static void RunClusterService(this IServiceProvider provider, string path = null, string key = null)
        {
            var service = provider.GetClusterService();
            service.ReadConfig(path, key);
            service.Run();
        }
    }
}
