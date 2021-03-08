using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

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
        /// <param name="path"></param>
        /// <param name="key"></param>
        /// <param name="services"></param>
        public static void RegisterRPCService(this ServiceHost host, string path = null, string key = null, params Type[] services)
        {
            host.RegisterType<RPCService>().SingleInstance();
            host.OnInitializing += provider =>
            {
                var service = provider.GetRPCService();
                services.ForEach(item =>
                {
                    service.AddInternalService(services => services.AddTransient(item));
                    service.AddRpcImplement(item);
                });
            };
            host.OnInitialized += async provider => await provider.RunRPCServiceAsync(path, key);
        }

        /// <summary>
        /// 获取服务
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static RPCService GetRPCService(this IServiceProvider provider)
        {
            return (RPCService)provider.GetService(typeof(RPCService));
        }

        /// <summary>
        /// 运行服务
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="path"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static async Task RunRPCServiceAsync(this IServiceProvider provider, string path = null, string key = null)
        {
            var service = provider.GetRPCService();
            service.ReadConfig(path, key);
            await service.RunAsync();
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        /// <param name="provider"></param>
        public static void StopRpcService(this IServiceProvider provider)
        {
            var service = provider.GetRPCService();
            service.Stop();
        }
    }
}
