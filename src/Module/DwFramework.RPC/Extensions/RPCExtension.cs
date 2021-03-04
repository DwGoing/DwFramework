using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Autofac;
using DwFramework.Core;
using DwFramework.Core.Extensions;

namespace DwFramework.RPC
{
    public static class RPCExtension
    {
        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="host"></param>
        /// <param name="configKey"></param>
        /// <param name="configPath"></param>
        /// <param name="services"></param>
        public static void RegisterRPCService(this ServiceHost host, string configKey = null, string configPath = null, params Type[] services)
        {
            host.Register(c => new RPCService(configKey, configPath, c.Resolve<ILogger<RPCService>>())).SingleInstance();
            host.OnInitializing += provider =>
            {
                var service = provider.GetRPCService();
                services.ForEach(item => service.AddService(item));
            };
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
