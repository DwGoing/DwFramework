using System;
using System.Threading.Tasks;
using Autofac;

using DwFramework.Core;

namespace DwFramework.Rpc
{
    public static class RpcExtension
    {
        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="host"></param>
        /// <param name="configFilePath"></param>
        public static void RegisterRpcService(this ServiceHost host, string configFilePath = null)
        {
            if (!string.IsNullOrEmpty(configFilePath))
            {
                host.AddJsonConfig(configFilePath);
                host.RegisterType<RpcService>().SingleInstance();
            }
            else host.Register(c => new RpcService(c.Resolve<Core.Environment>(), "Rpc")).SingleInstance();
            host.OnInitializing += provider => provider.InitRpcServiceAsync().Wait();
        }

        /// <summary>
        /// 获取服务
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static RpcService GetRpcService(this IServiceProvider provider) => provider.GetService<RpcService>();

        /// <summary>
        /// 初始化服务
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static Task InitRpcServiceAsync(this IServiceProvider provider) => provider.GetRpcService().OpenServiceAsync();
    }
}
