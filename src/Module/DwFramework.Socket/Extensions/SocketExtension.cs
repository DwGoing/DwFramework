using System;
using System.Threading.Tasks;

using DwFramework.Core;

namespace DwFramework.Socket
{
    public static class SocketExtension
    {
        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="host"></param>
        /// <param name="configKey"></param>
        /// <param name="configPath"></param>
        public static void RegisterSocketService(this ServiceHost host, string configKey = null, string configPath = null)
        {
            host.Register(c => new SocketService(configKey, configPath)).SingleInstance();
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
