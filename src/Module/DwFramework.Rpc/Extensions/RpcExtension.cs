using System;
using System.Threading.Tasks;
using Autofac;

using DwFramework.Core;

namespace DwFramework.RPC
{
    public static class RPCExtension
    {
        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="host"></param>
        /// <param name="configFilePath"></param>
        public static void RegisterRPCService(this ServiceHost host, string configFilePath = null)
        {
            if (!string.IsNullOrEmpty(configFilePath))
            {
                host.AddJsonConfig(configFilePath, "RPC");
                host.RegisterType<RPCService>().SingleInstance();
            }
            else host.Register(c => new RPCService(c.Resolve<Core.Environment>(), "RPC")).SingleInstance();
            host.OnInitializing += async provider => await provider.InitRPCServiceAsync();
        }

        /// <summary>
        /// 获取服务
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static RPCService GetRPCService(this IServiceProvider provider) => provider.GetService<RPCService>();

        /// <summary>
        /// 初始化服务
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static Task InitRPCServiceAsync(this IServiceProvider provider) => provider.GetRPCService().OpenServiceAsync();
    }
}
