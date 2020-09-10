using System;
using System.Threading.Tasks;

using DwFramework.Core;

namespace DwFramework.Rpc.Extensions
{
    public static class RpcExtension
    {
        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="host"></param>
        public static void RegisterRpcService(this ServiceHost host)
        {
            host.RegisterType<RpcService>().SingleInstance();
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
