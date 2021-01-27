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
        /// <param name="path"></param>
        /// <param name="key"></param>
        public static void RegisterSocketService(this ServiceHost host, string path = null, string key = null)
        {
            host.Register(_ => new SocketService(path, path)).SingleInstance();
            host.OnInitializing += provider => provider.InitSocketServiceAsync().Wait();
        }

        /// <summary>
        /// 获取服务
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static SocketService GetSocketService(this IServiceProvider provider)
        {
            return provider.GetService<SocketService>();
        }

        /// <summary>
        /// 初始化服务
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static Task InitSocketServiceAsync(this IServiceProvider provider)
        {
            return provider.GetSocketService().OpenServiceAsync();
        }
    }
}
