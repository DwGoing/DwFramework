using System;
using System.Net.Sockets;
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

        /// <summary>
        /// 启用keep-alive
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="intervalStart"></param>
        /// <param name="retryInterval"></param>
        public static void EnableKeepAlive(this System.Net.Sockets.Socket socket, uint keepAliveTime, uint interval)
        {
            var size = sizeof(uint);
            var inArray = new byte[size * 3];
            Array.Copy(BitConverter.GetBytes(1u), 0, inArray, 0, size);
            Array.Copy(BitConverter.GetBytes(keepAliveTime), 0, inArray, size, size);
            Array.Copy(BitConverter.GetBytes(interval), 0, inArray, size * 2, size);
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, inArray);
        }
    }
}
