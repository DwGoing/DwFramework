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
        /// <param name="config"></param>
        public static void RegisterTcpService(this ServiceHost host, TcpService.Config config)
        {
            host.RegisterType<TcpService>().SingleInstance();
            host.OnInitialized += async provider => await provider.RunTcpServiceAsync(config);
        }

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="host"></param>
        /// <param name="path"></param>
        /// <param name="key"></param>
        public static void RegisterTcpService(this ServiceHost host, string path = null, string key = null)
        {
            host.RegisterType<TcpService>().SingleInstance();
            host.OnInitialized += async provider => await provider.RunTcpServiceAsync(path, key);
        }

        /// <summary>
        /// 获取服务
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static TcpService GetTcpService(this IServiceProvider provider)
        {
            return provider.GetService<TcpService>();
        }

        /// <summary>
        /// 运行服务
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static async Task RunTcpServiceAsync(this IServiceProvider provider, TcpService.Config config)
        {
            var service = provider.GetTcpService();
            service.ReadConfig(config);
            await service.RunAsync();
        }

        /// <summary>
        /// 运行服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="provider"></param>
        /// <param name="path"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static async Task RunTcpServiceAsync(this IServiceProvider provider, string path = null, string key = null)
        {
            var service = provider.GetTcpService();
            service.ReadConfig(path, key);
            await service.RunAsync();
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        /// <param name="provider"></param>
        public static void StopTcpService(this IServiceProvider provider)
        {
            var service = provider.GetTcpService();
            service.Stop();
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

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="host"></param>
        /// <param name="config"></param>
        public static void RegisterUdpService(this ServiceHost host, UdpService.Config config)
        {
            host.RegisterType<UdpService>().SingleInstance();
            host.OnInitialized += async provider => await provider.RunUdpServiceAsync(config);
        }

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="host"></param>
        /// <param name="path"></param>
        /// <param name="key"></param>
        public static void RegisterUdpService(this ServiceHost host, string path = null, string key = null)
        {
            host.RegisterType<UdpService>().SingleInstance();
            host.OnInitialized += async provider => await provider.RunUdpServiceAsync(path, key);
        }

        /// <summary>
        /// 获取服务
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static UdpService GetUdpService(this IServiceProvider provider)
        {
            return provider.GetService<UdpService>();
        }

        /// <summary>
        /// 运行服务
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static async Task RunUdpServiceAsync(this IServiceProvider provider, UdpService.Config config)
        {
            var service = provider.GetUdpService();
            service.ReadConfig(config);
            await service.RunAsync();
        }

        /// <summary>
        /// 运行服务
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static async Task RunUdpServiceAsync(this IServiceProvider provider, string path = null, string key = null)
        {
            var service = provider.GetUdpService();
            service.ReadConfig(path, key);
            await service.RunAsync();
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        /// <param name="provider"></param>
        public static void StopUdpService(this IServiceProvider provider)
        {
            var service = provider.GetUdpService();
            service.Stop();
        }
    }
}
