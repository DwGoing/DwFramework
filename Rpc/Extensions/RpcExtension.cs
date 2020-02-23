using System;
using System.Threading.Tasks;

using DwFramework.Core;
using DwFramework.Core.Extensions;

namespace DwFramework.Rpc.Extensions
{
    public static class RpcExtension
    {
        /// <summary>
        /// 注册Rpc服务
        /// </summary>
        /// <param name="host"></param>
        public static void RegisterRpcService(this ServiceHost host)
        {
            host.RegisterType<IRpcService, RpcService>().SingleInstance();
        }

        /// <summary>
        /// 初始化Rpc服务
        /// </summary>
        /// <param name="provider"></param>
        public static Task InitRpcServiceAsync(this IServiceProvider provider)
        {
            return provider.GetService<RpcService>().OpenServiceAsync();
        }
    }
}
