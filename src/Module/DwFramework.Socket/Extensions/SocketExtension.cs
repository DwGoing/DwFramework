using System;
using System.Threading.Tasks;
using Autofac;

using DwFramework.Core;

namespace DwFramework.Socket
{
    public static class SocketExtension
    {
        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="host"></param>
        /// <param name="configFilePath"></param>
        public static void RegisterSocketService(this ServiceHost host, string configFilePath = null)
        {
            if (!string.IsNullOrEmpty(configFilePath))
            {
                host.AddJsonConfig(configFilePath);
                host.RegisterType<SocketService>().SingleInstance();
            }
            else host.Register(c => new SocketService(c.Resolve<Core.Environment>(), "Socket")).SingleInstance();
            host.OnInitializing += provider => provider.InitSocketServiceAsync().Wait();
        }

        /// <summary>
        /// 获取服务
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static SocketService GetSocketService(this IServiceProvider provider) => provider.GetService<SocketService>();

        /// <summary>
        /// 初始化服务
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static Task InitSocketServiceAsync(this IServiceProvider provider) => provider.GetSocketService().OpenServiceAsync();
    }
}
